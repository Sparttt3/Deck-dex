using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ID : MonoBehaviour
{
    public string IDs;
    public LoadSerie loadserie;
    public Toggle toggle;
    public TextMeshProUGUI IDS_text;

    void Awake()
    {
        IDs = gameObject.name.ToString();
        loadserie = GameObject.FindAnyObjectByType<LoadSerie>();

        // Trouver le bouton et ajouter un écouteur d'événements pour appeler Selector() lorsque le bouton est cliqué
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(Selector);
        }
    }

    public void Selector()
    {
        loadserie.selectedCardIndex = int.Parse(IDs);
        loadserie.Updatecardssettings();
        loadserie.UpdateToggle();
        loadserie.Cardsettings.SetActive(true);
        loadserie.apiManager.LoadCard_Price();
        loadserie.scrollViewController.UpdateCartes();
    }
    public void Toggle()
    {
        loadserie.selectedCardIndex = int.Parse(IDs);
        loadserie.SavePokemon.UpdateVar();
        if (toggle.isOn == true)
        {
            loadserie.SavePokemon.isUnlock = "true";
        }
        else
        {
            loadserie.SavePokemon.isUnlock = "false";
        }
        loadserie.SavePokemon.UpdateCardsStatus();
    }
}