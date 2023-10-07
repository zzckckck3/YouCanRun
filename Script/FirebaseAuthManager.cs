using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using Firebase.Database;
using System;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using UnityEngine.UI;
using Firebase;
using Firebase.Extensions;
using TMPro;
using System.Linq.Expressions;

public class FirebaseAuthManager : MonoBehaviour
{
    public bool IsFirebaseReady { get; private set; }
    public bool IsSignInOnProgress { get; private set; }

    public TMP_InputField idField;
    public TMP_InputField passwordField;
    public Button signInButton;
    public GameObject LoadingRotate;
    public GameObject LoginCompletePopup;
    public GameObject LoginFailPopup;
    // 회원가입 popup
    public GameObject CreateCompletePopup;
    public GameObject CreateFailPopup;
    public TextMeshProUGUI failInfo;
    public GameObject LoadingRotate2;

    public static FirebaseApp firebaseApp;
    public static FirebaseAuth firebaseAuth;
    public static FirebaseUser User;

    private DatabaseReference databaseReference;
    public Action<bool> LoginState;

    private UserData userData;

    public List<UserRank> userRanks;
    public class UserData
    {
        public string userId;
        public string nickname;
        public int level;
        public long point;
        public string color;
        public string head;
        public string body;
        public int isFirstPlay;

        public UserData(string userId, string nickname, int level, long point, string color,
                        string head, string body, int isFirstPlay)
        {
            this.userId = userId;
            this.nickname = nickname;
            this.level = level;
            this.point = point;
            this.color = color;
            this.head = head;
            this.body = body;
            this.isFirstPlay = isFirstPlay;
        }
    }
    public class UserRank
    {
        public string nickname;
        public int level;
        public UserRank(string nickname, int level)
        {
            this.nickname = nickname;
            this.level = level;
        }
    }
    
    private static FirebaseAuthManager instance = null;
    public static FirebaseAuthManager Instance
    {
        get { return instance; }
    }

    public void Awake()
    {
        if (instance == null)
        {
            // 이 GameManager 오브젝트가 다른 씬으로 전환될 때 파괴되지 않도록 함
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else
        {
            // 이미 존재하는 GameManager 오브젝트가 있으므로 이 인스턴스를 파괴
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        firebaseAuth.SignOut();
        signInButton.interactable = false;
        
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                var result = task.Result;

                if (result != DependencyStatus.Available)
                {
                    Debug.LogError(result.ToString());
                    IsFirebaseReady = false;
                }
                else
                {
                    IsFirebaseReady = true;

                    firebaseApp = FirebaseApp.DefaultInstance;
                    firebaseAuth = FirebaseAuth.DefaultInstance;
                    // firebaseAuth = FirebaseAuth.GetAuth(firebaseApp); >> 이것도 되긴 된다
                }

                signInButton.interactable = IsFirebaseReady;
            }
        );
        //RetrieveUserData();
    }

    public void Init()
    {
        firebaseAuth = FirebaseAuth.DefaultInstance;
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        firebaseAuth.StateChanged += OnChanged;
    }

    private void OnChanged(object sender, EventArgs e)
    {
        if (firebaseAuth.CurrentUser != User)
        {
            bool signed = (firebaseAuth.CurrentUser != User && firebaseAuth.CurrentUser != null);
            if (!signed && User != null)
            {
                Debug.Log("로그아웃");
                LoginState?.Invoke(false);
            }

            User = firebaseAuth.CurrentUser;
            if (signed)
            {
                Debug.Log("로그인");
                LoginState?.Invoke(true);
            }
        }
    }

    public void RegisterUserData(string userId, string nickname)
    {
        var userData = new UserData(userId, nickname, 0, 0, "1", "06", "06",0);
        string jsonData = JsonUtility.ToJson(userData);

        databaseReference.Child("users").Child(firebaseAuth.CurrentUser.UserId).SetRawJsonValueAsync(jsonData)
            .ContinueWith(setTask =>
            {
                if (setTask.IsFaulted)
                {
                    foreach (var exception in setTask.Exception.Flatten().InnerExceptions)
                    {
                        Debug.LogError("데이터 저장 실패: " + exception.ToString());
                    }
                }
                else if (setTask.IsCompleted)
                {
                    Debug.Log("데이터 저장 완료");
                }
            });

    }

    public void Create(string id, string password, string nickname)
    {
        LoadingRotate2.SetActive(true);

        firebaseAuth.CreateUserWithEmailAndPasswordAsync(id + "@a.com", password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                LoadingRotate2.SetActive(false);
                CreateFailPopup.SetActive(true);
                Debug.LogError("회원가입 취소 : " + task.Exception );
                failInfo.text = "알 수 없는 오류가 발생하였습니다. 재부팅 해주세요.";
            }
            else if (task.IsFaulted)
            {
                LoadingRotate2.SetActive(false);
                CreateFailPopup.SetActive(true);
                Debug.LogError("회원가입 실패 : "+ task.Exception.ToString());
                if(password.Length < 6)
                {
                    failInfo.text = "비밀번호는 6자리 이상 입력해 주세요!";
                }
                else
                {
                    failInfo.text = "이미 사용중인 아이디 입니다.";
                }
            }
            else
            {
                //FirebaseUser newUser = task.Result.User;
                Debug.Log("회원가입 완료");
                LoadingRotate2.SetActive(false);
                CreateCompletePopup.SetActive(true);
                RegisterUserData(id, nickname);
            }
        });
    }
   
    public void Login() // string id, string password
    {
        User = null;

        if (!IsFirebaseReady || IsSignInOnProgress || User != null)
        {
            return;
        }

        LoadingRotate.SetActive(true);
        IsSignInOnProgress = true;
        signInButton.interactable = false;

        firebaseAuth.SignInWithEmailAndPasswordAsync(idField.text + "@a.com", passwordField.text).ContinueWithOnMainThread(task =>
        {
            Debug.Log($"Sign in status : {task.Status}");

            IsSignInOnProgress = false;
            signInButton.interactable = true;

            if (task.IsFaulted)
            {
                LoadingRotate.SetActive(false);
                LoginFailPopup.SetActive(true);
                Debug.LogError(task.Exception);
            } 
            else if (task.IsCanceled)
            {
                LoadingRotate.SetActive(false);
                LoginFailPopup.SetActive(true);
                Debug.LogError("Sign-in canceled");
            }
            else
            {
                User = task.Result.User;
                GameManager.Instance.userData = GetUserDataFromFireBase();
                Debug.Log(User.Email);
                LoadingRotate.SetActive(false);
                LoginCompletePopup.SetActive(true);
            }
        });
    }

    public void LogOut()
    {
        firebaseAuth.SignOut();
        Debug.Log("로그아웃");
    }

    public UserData GetUserDataFromFireBase()
    {
        databaseReference.Child("users").Child(firebaseAuth.CurrentUser.UserId).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.Log("로드 취소");
                return;
            }
            else if (task.IsFaulted)
            {
                Debug.Log("로드 실패");
                return;
            }
            else
            {
                var dataSnapshot = task.Result;

                userData = JsonUtility.FromJson<UserData>(dataSnapshot.GetRawJsonValue());

            }

        });
        return userData;
    }

    public UserData GetUserData()
    {
        return userData;
    }

    public void UpdateUserDataToFireBase(string userId, string nickname, 
                                        int level, long point, string color, 
                                        string head, string body, int isFirstPlay)
    {
        UserData updatedUserData = new UserData(userId, nickname, level, point, color, head, body, isFirstPlay);
        string jsonData = JsonUtility.ToJson(updatedUserData);

        databaseReference.Child("users").Child(firebaseAuth.CurrentUser.UserId).SetRawJsonValueAsync(jsonData)
            .ContinueWith(setTask =>
            {
                if (setTask.IsFaulted)
                {
                    foreach (var exception in setTask.Exception.Flatten().InnerExceptions)
                    {
                        Debug.LogError("데이터 업데이트 실패: " + exception.ToString());
                    }
                }
                else if (setTask.IsCompleted)
                {
                    Debug.Log("데이터 업데이트 완료");
                }
            });
    }
    public List<UserRank> RetrieveUserData()
    {
        userRanks = new List<UserRank>();

        DatabaseReference usersRef = databaseReference.Child("users");

        usersRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("데이터를 가져오는 동안 오류 발생: " + task.Exception);
                return;
            }

            DataSnapshot snapshot = task.Result;
            foreach (DataSnapshot userSnapshot in snapshot.Children)
            {
                string userId = userSnapshot.Child("nickname").GetValue(true).ToString(); // 사용자의 닉네임
                int level = int.Parse(userSnapshot.Child("level").GetValue(true).ToString()); // 사용자의 "level" 데이터
                
                userRanks.Add(new UserRank(userId, level));
                //Debug.Log("User ID: " + userId + ", level: " + level);
            }
        });
        return userRanks;
    }

    public void UpdateNicknameToFireBase(string nickname)
    {
        databaseReference.Child("users").Child(firebaseAuth.CurrentUser.UserId).Child("nickname").SetValueAsync(nickname);
    }

    public async Task<bool> UpdateLevelToFireBase(int level)
    {
        try
        {
            await databaseReference.Child("users")
                .Child(firebaseAuth.CurrentUser.UserId)
                .Child("level")
                .SetValueAsync(level);

            // 작업이 성공적으로 완료됨
            return true;
        }
        catch (Exception e)
        {
            // 작업이 실패함
            Debug.LogError("값 설정 중 오류 발생: " + e.Message);
            return false;
        }
    }

    public void UpdatePointToFireBase(long point)
    {
        databaseReference.Child("users").Child(firebaseAuth.CurrentUser.UserId).Child("point").SetValueAsync(point);
    }

    public void UpdateColorToFireBase(string color)
    {
        databaseReference.Child("users").Child(firebaseAuth.CurrentUser.UserId).Child("color").SetValueAsync(color);
    }

    public void UpdateHeadToFireBase(string head)
    {
        databaseReference.Child("users").Child(firebaseAuth.CurrentUser.UserId).Child("head").SetValueAsync(head);
    }

    public void UpdateBodyToFireBase(string body)
    {
        databaseReference.Child("users").Child(firebaseAuth.CurrentUser.UserId).Child("body").SetValueAsync(body);
    }

    public async Task<bool> UpdateIsFirstPlayToFireBase(int isFirstPlay)
    {
        try
        {
            await databaseReference.Child("users")
                .Child(firebaseAuth.CurrentUser.UserId)
                .Child("isFirstPlay")
                .SetValueAsync(isFirstPlay);

            // 작업이 성공적으로 완료됨
            return true;
        }
        catch (Exception e)
        {
            // 작업이 실패함
            Debug.LogError("값 설정 중 오류 발생: " + e.Message);
            return false;
        }
    }
}