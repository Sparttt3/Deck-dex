using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PhoneCam : MonoBehaviour
{
    private bool camAvailable;
    private WebCamTexture backCam;
    private Texture defaultBackground;

    public RawImage background;
    public AspectRatioFitter fit;

    private void Start()
    {
        defaultBackground = background.texture;
        WebCamDevice[] devices = WebCamTexture.devices;

        if(devices.Length == 0 )
        {
            Debug.Log("Pas de camera détécter");
            camAvailable = false;
            return;
        }
        for(int i=0; i<devices.Length; i++) {
            if(!devices[i].isFrontFacing)
            {
                backCam = new WebCamTexture(devices[i].name);
            }
        }
        if(backCam == null)
        {
            Debug.Log("unable to find back camera");
            return;
        }
        backCam.Play();
        background.texture = backCam;

        camAvailable = true;
    }

    private void Update()
    {
        if(!camAvailable)
        {
            return;
        }
        float ration = (float)backCam.width / (float)backCam.height;
        fit.aspectRatio = ration;

        background.rectTransform.localScale = new Vector3(1f, 1f, 1f);

        int orient = -backCam.videoRotationAngle;
        background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);
    }
    public void ScreenShot()
    {
        if (!camAvailable)
        {
            Debug.Log("Caméra non disponible");
            return;
        }

        // Crée une texture pour capturer les données actuelles de la caméra
        Texture2D snap = new Texture2D(backCam.width, backCam.height, TextureFormat.RGB24, false);
        snap.SetPixels(backCam.GetPixels());
        snap.Apply();

        // Convertir la texture en PNG
        byte[] bytes = snap.EncodeToPNG();

        // Chemin du fichier de sortie
        string filePath = Path.Combine(Application.persistentDataPath, "screen_shot2.png");
        Debug.LogError(filePath);

        // Sauvegarder sur le disque
        File.WriteAllBytes(filePath, bytes);

        Debug.Log("Capture sauvegardée : " + filePath);
    }
}