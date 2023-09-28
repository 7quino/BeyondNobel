using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NativeShare : MonoBehaviour
{
    public string shareTitle = "Share Image";
    public string shareText = "Check out this image!";
    public TextMeshProUGUI debugText;

    public void Share(Texture2D textureToShare)
    {
        debugText.text = "\n button pressed";

        try
        {
            byte[] imageBytes = textureToShare.EncodeToPNG();

            // Save the byte array to a temporary file
            string imagePath = Path.Combine(Application.temporaryCachePath, "shared_image.png");
            File.WriteAllBytes(imagePath, imageBytes);

            // Create intent for sharing
            AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
            AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");

            // Set action to send
            intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));

            // Set type of data
            intentObject.Call<AndroidJavaObject>("setType", "image/*");

            // Attach the image file to the intent
            AndroidJavaObject uriObject = new AndroidJavaObject("android.net.Uri", imagePath);
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);

            // Set the title and text
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TITLE"), shareTitle);
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), shareText);

            // Get the current Unity activity
            AndroidJavaClass unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");

            // Start the chooser activity
            AndroidJavaObject chooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, shareTitle);
            currentActivity.Call("startActivity", chooser);

            debugText.text = "\n share ok";
        }
        catch (Exception e)
        {
            debugText.text = "\n " + e.ToString();
        }
    }
}
