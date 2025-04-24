using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SavePokemon : MonoBehaviour
{
    public string isUnlock;
    public string Multiplecards;

    public RectTransform content;

    public LoadSerie LoadSerie;

    public TextMeshProUGUI Calac_Progress_txt;
    public TextMeshProUGUI Progress_txt;
    public TMP_InputField NomberInputeField;

    void ResetContentpos()
    {
        Vector2 offset = content.offsetMax; // Récupère les offsets actuels
        offset.y = 0; // Définit la nouvelle valeur pour 'bottom'
        content.offsetMax = offset; // Applique les changements
    }
    public void ActiveAllCards()
    {
        for (int i = 1; i < LoadSerie.rawImage.Length + 1; i++)
        {
            if(LoadSerie.rawImage[i - 1] != null)
            {
                LoadSerie.rawImage[i - 1].gameObject.SetActive(true);
            }
        }
        ResetContentpos();
    }
    //Charger les carte non débloquer uniquement
    public void LoadOnlyLockedCards()
    {
        string Display_Save = "";

        for (int i = 1; i < LoadSerie.appManager.Nombre_de_carte + 1; i++)
        {
            Display_Save = PlayerPrefs.GetString(LoadSerie.appManager.Pokemon_serie + i, "Valeur par défaut si non trouvée");

            if (Display_Save == "Valeur par défaut si non trouvée" || Display_Save == "false" && LoadSerie.rawImage[i - 1] != null)
            {
                LoadSerie.rawImage[i - 1].gameObject.SetActive(true);
            }
            else
            {
                LoadSerie.rawImage[i - 1].gameObject.SetActive(false);
            }
        }
        ResetContentpos();
    }

    //Charger les carte débloquer uniquement
    public void LoadOnlyUnlockCards()
    {
        string Display_Save = "";

        for (int i = 1; i < LoadSerie.appManager.Nombre_de_carte + 1; i++)
        {
            Display_Save = PlayerPrefs.GetString(LoadSerie.appManager.Pokemon_serie + i, "Valeur par défaut si non trouvée");

            if (Display_Save == "Valeur par défaut si non trouvée" || Display_Save == "false" && LoadSerie.rawImage[i - 1] != null)
            {
                LoadSerie.rawImage[i - 1].gameObject.SetActive(false);
            }
            else
            {
                if(Display_Save == "true")
                {
                    LoadSerie.rawImage[i - 1].gameObject.SetActive(true);
                }
            }
        }
        ResetContentpos();
    }
    public void UpdateCardsStatus() //Actualiser les carte débloquer ou non plus le nombre de cartes posséder
    {
        SaveGameData();
        string Display_Save = "";
        string Display_Number = "";
        int Progress = 0;

        for (int i = 1; i < LoadSerie.appManager.Nombre_de_carte+1; i++)
        {
            Display_Save = PlayerPrefs.GetString(LoadSerie.appManager.Pokemon_serie + i, "");
            Display_Number = PlayerPrefs.GetString(LoadSerie.appManager.Pokemon_serie + i+"Number", "");

            //Mettre le nombre de cartes présent
            if(LoadSerie.rawImage[i - 1] != null)
            {
                if (Display_Number != "" && Display_Number != "0")
                {
                    LoadSerie.rawImage[i - 1].GetComponentInChildren<TextMeshProUGUI>().text = "x" + Display_Number;
                }
                else
                {
                    LoadSerie.rawImage[i - 1].GetComponentInChildren<TextMeshProUGUI>().text = null;
                }
                //Enlever le gris sur les cartes
                if (Display_Save == "true")
                {
                    LoadSerie.rawImage[i - 1].GetComponentInChildren<Toggle>().isOn = true;
                    LoadSerie.rawImage[i - 1].color = Color.white;
                    isUnlock = "true";
                    Progress++;
                }
                else
                {
                    LoadSerie.rawImage[i - 1].GetComponentInChildren<Toggle>().isOn = false;
                    LoadSerie.rawImage[i - 1].color = new Color(0.7f, 0.7f, 0.7f, 15);
                    isUnlock = "false";
                }
            }
            continue;
        }
        // calcule le pousantage de carte obtenue sur le total de carte.
        int result = (int)(((float)Progress / LoadSerie.appManager.Nombre_de_carte) * 100);
        Progress_txt.text = result + "%";
        Calac_Progress_txt.text = Progress + "/" + LoadSerie.appManager.Nombre_de_carte;
    }
    public void UpdateVar() // actualise les variable pour actualiser l'ui
    {
        isUnlock = PlayerPrefs.GetString(LoadSerie.appManager.Pokemon_serie + LoadSerie.selectedCardIndex, "Valeur par défaut si non trouvée");
        Multiplecards = PlayerPrefs.GetString(LoadSerie.appManager.Pokemon_serie + LoadSerie.selectedCardIndex+"Number", "0");
        NomberInputeField.text = Multiplecards;
    }
    public void CardNumber() // Sauvegarder le text apartire du inputeField
    {
        Multiplecards = NomberInputeField.text;
        SaveGameData();
    }
    // Méthode pour sauvegarder les données du joueur
    public void SaveGameData()
    {
        PlayerPrefs.SetString(LoadSerie.appManager.Pokemon_serie + LoadSerie.selectedCardIndex.ToString(), isUnlock);
        PlayerPrefs.SetString(LoadSerie.appManager.Pokemon_serie + LoadSerie.selectedCardIndex.ToString()+"Number", Multiplecards);
        PlayerPrefs.Save(); // Sauvegarde immédiate des données sur le disque
    }

    // Méthode pour charger les données du joueur et retourner la valeur
    public string LoadGameData()
    {
        return PlayerPrefs.GetString(LoadSerie.appManager.Pokemon_serie + LoadSerie.selectedCardIndex.ToString(), "Valeur par défaut si non trouvée");
    }
}