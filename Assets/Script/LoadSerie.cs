using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System;
using UnityEngine.Rendering;


public class LoadSerie : MonoBehaviour
{
    [SerializeField] public RawImage[] rawImage;
    public RawImage SelectcardsImage; //Image de Cardsettings
    public RawImage CardsPref; // est appelé depuis ID.script


    public GameObject Cardsettings; // est appelé depuis ID.script
    public GameObject Loadingobj; // est appelé depuis ID.script

    public TextMeshProUGUI CardNumber; // Dans le Cardsettings
    public TextMeshProUGUI Card_Price; // Dans le Cardsettings
    public TextMeshProUGUI LoadingScreen_Status; // Dans le Cardsettings

    public int selectedCardIndex; // index de la carte selectionner

    public Toggle Unlock_Toggle; // Toggle dans cardsettings

    public SavePokemon SavePokemon;
    public ScrollViewController scrollViewController;
    public ApiManager apiManager;
    public AppManager appManager;
    string seriesFolder;

    public Texture2D defaultTexture; // Image par défaut

    public void Start_script()
    {
        // Boucler à l'envers pour éviter des problèmes liés à la modification de la hiérarchie.
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        seriesFolder = Path.Combine(Application.persistentDataPath, appManager.Pokemon_serie);
        rawImage = new RawImage[appManager.Nombre_de_carte];  // Tableau de 10 chaînes de caractères
        StartCoroutine(DownloadImages());
    }
    private IEnumerator DownloadImages()
    {
        appManager.Loading_screen_is_enable = true;
        LoadingScreen_Status.text = "Loading...";
        for (int i = 0; i < appManager.Nombre_de_carte; i++)
        {
            string imageName = (i + 1) + appManager.image_url_fin;
            string imagePath = Path.Combine(seriesFolder, imageName);
            string currentImageUrl = appManager.image_url_debut + imageName;

            if (!File.Exists(imagePath))
            {
                LoadingScreen_Status.text = "Downloading...";
                UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(currentImageUrl);
                yield return uwr.SendWebRequest();

                // Vérifier si le téléchargement a échoué
                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    // Enregistrer l'image uniquement si le téléchargement a réussi
                    Directory.CreateDirectory(seriesFolder); // Crée le répertoire seulement si nécessaire
                    File.WriteAllBytes(imagePath, uwr.downloadHandler.data);
                }
                else
                {
                    // Appliquer l'image par défaut si le téléchargement échoue
                    if (defaultTexture != null)
                    {
                        // Charger la texture par défaut
                        RawImage instanceObject = Instantiate(CardsPref);
                        instanceObject.transform.SetParent(transform, false);
                        instanceObject.texture = defaultTexture;
                        instanceObject.GetComponent<ID>().IDs = (1 + i).ToString();
                        instanceObject.GetComponent<ID>().IDS_text.text = "#" + (1 + i).ToString();
                        rawImage[i] = instanceObject;
                        appManager.Loadingscreen(i);
                        Debug.LogError("Image par défaut définie !");
                    }
                    continue; // Passer cette image et continuer avec la suivante
                }
            }
            // Charger la texture depuis le fichier local de manière asynchrone
            yield return StartCoroutine(LoadTextureAsync(imagePath, i));
        }
        // Fermer la fenêtre de chargement
        Loadingobj.SetActive(false);
        appManager.Loading_screen_is_enable = false;
        // Mettre à jour les cartes
        SavePokemon.UpdateCardsStatus();
    }

    private IEnumerator LoadTextureAsync(string path, int index)
    {
        byte[] fileData = null;

        // Lire les données du fichier de manière asynchrone
        yield return StartCoroutine(ReadFileAsync(path, result => fileData = result));
        if (fileData == null) yield break;

        Texture2D texture = new Texture2D(0, 0);
        texture.LoadImage(fileData);

        //affecter la texture au composant RawImage
        RawImage instanceObject = Instantiate(CardsPref);
        instanceObject.transform.SetParent(transform, false);
        instanceObject.texture = texture;
        instanceObject.GetComponent<ID>().IDs = (1 + index).ToString();
        instanceObject.GetComponent<ID>().IDS_text.text = "#" + (1 + index).ToString();
        rawImage[index] = instanceObject;
        appManager.Loadingscreen(index);
    }
    private IEnumerator ReadFileAsync(string path, System.Action<byte[]> callback)
    {
        byte[] fileData = null;

        if (File.Exists(path))
        {
            // Lire le fichier de manière asynchrone
            var task = File.ReadAllBytesAsync(path);

            // Attendre que la tâche se termine sans bloquer le thread principal
            while (!task.IsCompleted)
            {
                yield return null;
            }

            // Vérifier si la tâche a échoué
            if (task.IsFaulted)
            {
                Debug.LogError($"Erreur lors de la lecture du fichier : {path}");
            }
            else
            {
                fileData = task.Result;
            }
        }
        // Appeler le callback, même si fileData est null
        callback(fileData);
    }

    public IEnumerator DownloadAndApplyTexture(string url, RawImage image)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
            UnityWebRequestAsyncOperation asyncOp = uwr.SendWebRequest();

            while (!asyncOp.isDone)
            {
                yield return null;  // Attendre que la requête soit terminée sans bloquer le thread principal
            }

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Erreur lors du téléchargement de {url}: {uwr.error}");

                // Appliquer l'image par défaut si le téléchargement échoue
                if (defaultTexture != null)
                {
                    image.texture = defaultTexture; // Appliquer l'image par défaut
                }
                else
                {
                    Debug.LogError("Aucune image par défaut définie !");
                }
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
                image.texture = texture; // Appliquer la texture téléchargée
            }
        }
    }

    public void Updatecardssettings() // Charge l'image quand Cardsettings est ouvert
    {
        SelectcardsImage.texture = rawImage[selectedCardIndex - 1].texture;
        CardNumber.text = appManager.Pokemon_serie + selectedCardIndex;
    }

    public void UpdateToggle() // Met à jour la totale quand la carte est ouverte 
    {
        SavePokemon.UpdateVar();
        if (SavePokemon.isUnlock == "true")
        {
            Unlock_Toggle.isOn = true;
        }
        else
        {
            Unlock_Toggle.isOn = false;
        }
    }

    public void Togle() // Change la valeur du toggle sur la carte avant prochain changement
    {
        if (Unlock_Toggle.isOn == true)
        {
            SavePokemon.isUnlock = "true";
        }
        else
        {
            SavePokemon.isUnlock = "false";
        }
    }
}