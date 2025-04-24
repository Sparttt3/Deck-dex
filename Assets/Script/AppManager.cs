using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem.HID;


[System.Serializable]
public class Traduction
{
    public Image[] buttonImage;
    public Texture2D[] imageFR;
    public Texture2D[] imageEN;
    public void read_data()
    {
        // lecture des donnes
        bool fr = PlayerPrefs.GetInt("language:fr", 0) == 1;
        bool en = PlayerPrefs.GetInt("language:en", 0) == 1;

        for (int i = 0; i < buttonImage.Length; i++)
        {
            if (PlayerPrefs.GetInt("language:fr", 0) == 1)
            {
                fr = true;
                en = false;
                Sprite newSprite = Sprite.Create(imageFR[i],
                new Rect(0, 0, imageFR[i].width, imageFR[i].height),
                new Vector2(0.5f, 0.5f));

                buttonImage[i].sprite = newSprite;
            }
            else
            {
                if (PlayerPrefs.GetInt("language:en", 0) == 1)
                {
                    en = true;
                    fr = false;
                    Sprite newSprite = Sprite.Create(imageEN[i],
                    new Rect(0, 0, imageEN[i].width, imageEN[i].height),
                    new Vector2(0.5f, 0.5f));

                    buttonImage[i].sprite = newSprite;
                }
            }
        }
    }
}
public class AppManager : MonoBehaviour
{
    public LoadSerie loadSerie;

    public int Nombre_de_carte;

    public string Pokemon_serie;
    public string image_url_debut;
    public string image_url_fin;
    public string api_url_debut;

    public Image LogoImage;

    public TextMeshProUGUI Progress;

    public RectTransform content;

    public bool Loading_screen_is_enable;
    public bool promo;
    public bool fr;
    public bool en;

    public GameObject[] Return_priority;

    public List<Traduction> trad;

    private void Start()
    {
        Application.targetFrameRate = 60;
        calltrad();
        Debug.Log(PlayerPrefs.GetInt("language:fr", 0) == 1);
        Debug.Log(PlayerPrefs.GetInt("language:en", 0) == 1);
    }
    public void calltrad()
    {
        foreach (var traduction in trad)
        {
            if (traduction != null)
            {
                traduction.read_data();
            }
        }
    }
    public void LanguageUpdate(int value)
    {
        switch (value) // 0 pour FR
        {
            case 0: // Option 1
                PlayerPrefs.SetInt("language:fr", 1);
                PlayerPrefs.SetInt("language:en", 0);
                break;
            case 1: // 1 pour EN
                PlayerPrefs.SetInt("language:en", 1);
                PlayerPrefs.SetInt("language:fr", 0);
                break;
        }
        // Sauvegarde dans PlayerPrefs
        PlayerPrefs.Save();
        calltrad();
        Debug.Log(value);
    }
    public void ReturnButton()
    {
        for (int i = 0; i < Return_priority.Length; i++)
        {
            // Recherche des enfants actifs
            foreach (Transform child in Return_priority[i].transform)
            {
                if (child.gameObject.activeSelf && !Loading_screen_is_enable) // Vérifie si l'enfant est actif
                {
                    child.gameObject.SetActive(false);
                    return; // Sort de la boucle après avoir trouvé le premier enfant actif
                }
            }
        }
    }
    public void Loadingscreen(int index)
    {
        Progress.text = index +"/"+ Nombre_de_carte.ToString();
    }
    public void OpenUI(GameObject obj)
    {
        obj.SetActive(true);
    }
    public void CloseUI(GameObject obj)
    {
        if(!Loading_screen_is_enable)
        {
            obj.SetActive(false);
        }
    }
    public void LoadScene(string Name)
    {
        SceneManager.LoadScene(Name);
    }
    public void ReloadScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
    public void NombreDeCarte(int ndc)
    {
        Nombre_de_carte = ndc;
    }
    public void PokemonSerie(string ps)
    {
        Pokemon_serie = ps;
    }
    public void UrlDebut(string ud)
    {
        image_url_debut = ud;
    }
    public void UrlFin(string uf)
    {
        image_url_fin = uf;
    }
    public void APIUrlDebut(string aud)
    {
        api_url_debut = aud;
    }
    public void SerieLogo(Sprite ls)
    {
        if(ls != null)
        {
            LogoImage.sprite = ls;
        }
        ChangeContentValue();
        loadSerie.Start_script();
    }
    public void isPromo(bool p)
    {
        if(p)
        {
            promo = true;
        }else
        {
            promo = false;
        }
    }
    void ChangeContentValue()
    {
        if (content != null)
        {
            Vector2 offset = content.offsetMin; // Récupère les offsets actuels
            offset.y = -((float)Nombre_de_carte*230f) - Nombre_de_carte; // Définit la nouvelle valeur pour 'bottom'
            content.offsetMin = offset; // Applique les changements
        }
    }
}
