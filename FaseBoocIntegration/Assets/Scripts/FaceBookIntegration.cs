using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Facebook.Unity;
using UnityEngine.UI;

public class FaceBookIntegration : MonoBehaviour
{
    public Text FriendText;
    public Text FB_userName;
    public Image FB_useerDp;

    /// <summary>
    /// инициализация пользователя при включении приложения
    /// </summary>
    private void Awake()
    {
        if (!FB.IsInitialized)
        {
            FB.Init(() =>
            {
                if (FB.IsInitialized)
                    FB.ActivateApp();
                else
                    Debug.LogError("Couldnt initialiaze");
            },
            isGameShown =>
            {
                if (!isGameShown)
                    Time.timeScale = 0;
                else
                    Time.timeScale = 1;
            });
        }
        else
            FB.ActivateApp();
    }

    #region Login / Logout
    public void FacebookLogin()
    {
        var permission = new List<string>() {"public_profile","email","user_friends"};
        FB.LogInWithReadPermissions(permission, AuthCallBack);
    }

    public void FacebookLogout()
    {
        FB.LogOut();
        StartCoroutine("FBLogout");
    }
    #endregion

    public void FacebookShare()
    {
        // ссылка на то что мы хотим отправить
        string url1 = "https://play.google.com/store/apps/details?id=com.AlexCatsStudios.StepHeros&fbclid=IwAR3igNnIfLFTJs9Yem6FU9DHoeu0VZ1Lfsb9Hxp2-NbVwgeLbaLNsqfTWTw";
        // ссылка на картинку лого
        string url2 = "https://wampi.ru/image/Rsf5AkQ";
        FB.ShareLink(new System.Uri(url1), "Check it out",
            "Good programming tutorials",
            new System.Uri(url2));
    }

    #region Inviting
    /// <summary>
    /// Реализация отправки приглашения другим пользователям FaceBook
    /// </summary>
    public void FacebookGameRequest()
    {
        FB.AppRequest("Hey! Come and play this awesome game!", title: "Pirate 3 math");
    }
    #endregion

    public void GetFriendsPlayingThisGame()
    {
        string query = "/me/friends";
        FB.API(query, HttpMethod.GET, result =>
        {
            var dictionary = (Dictionary<string, object>)Facebook.MiniJSON.Json.Deserialize(result.RawResult);
            var friendsList = (List<object>)dictionary["data"];
            FriendText.text = string.Empty;
            foreach (var dict in friendsList)
                FriendText.text += ((Dictionary<string, object>)dict)["name"];
        });

    }

   

    #region Courytine
  

    private static readonly string EVENT_PARAM_SCORE = "score";

    private static readonly string EVENT_NAME_GAME_PLAYED = "game_played";

   

    void SetInit()

    {

        if (FB.IsLoggedIn)

        {

            Debug.Log("Facebook is Login!");

        }

        else

        {

            Debug.Log("Facebook is not Logged in!");

        }

        DealWithFbMenus(FB.IsLoggedIn);

    }



    void onHidenUnity(bool isGameShown)

    {

        if (!isGameShown)

        {

            Time.timeScale = 0;

        }

        else

        {

            Time.timeScale = 1;

        }

    }

    


    IEnumerator FBLogout()

    {



        while (FB.IsLoggedIn)

        {

            print("Logging Out");

            yield return null;

        }

        print("Logout Successful");

        FB_useerDp.sprite = null;

        FB_userName.text = "";

    }





    

    private static void ShareCallback(IShareResult result)

    {

        Debug.Log("ShareCallback");

        SpentCoins(2, "sharelink");

        if (result.Error != null)

        {

            Debug.LogError(result.Error);

            return;

        }

        Debug.Log(result.RawResult);

    }

    // Start is called before the first frame update

    void AuthCallBack(IResult result)

    {

        if (result.Error != null)

        {

            Debug.Log(result.Error);

        }

        else

        {

            if (FB.IsLoggedIn)

            {

                Debug.Log("Facebook is Login!");

                // Panel_Add.SetActive(true);

            }

            else

            {

                Debug.Log("Facebook is not Logged in!");

            }

            DealWithFbMenus(FB.IsLoggedIn);

        }

    }



    void DealWithFbMenus(bool isLoggedIn)

    {

        if (isLoggedIn)

        {

            FB.API("/me?fields=first_name", HttpMethod.GET, DisplayUsername);

            FB.API("/me/picture?type=square&height=128&width=128", HttpMethod.GET, DisplayProfilePic);

        }

        else

        {



        }

    }

    void DisplayUsername(IResult result)

    {

        if (result.Error == null)

        {

            string name = "" + result.ResultDictionary["first_name"];

            FB_userName.text = name;



            Debug.Log("" + name);

        }

        else

        {

            Debug.Log(result.Error);

        }

    }



    void DisplayProfilePic(IGraphResult result)

    {

        if (result.Texture != null)

        {

            Debug.Log("Profile Pic");

            FB_useerDp.sprite = Sprite.Create(result.Texture, new Rect(0, 0, 128, 128), new Vector2());

        }

        else

        {

            Debug.Log(result.Error);

        }

    }

    public static void SpentCoins(int coins, string item)

    {

        // setup parameters

        var param = new Dictionary<string, object>();

        param[AppEventParameterName.ContentID] = item;

        // log event

        FB.LogAppEvent(AppEventName.SpentCredits, (float)coins, param);

    }
    #endregion
}
