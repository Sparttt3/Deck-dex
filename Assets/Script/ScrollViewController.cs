using UnityEngine;
using UnityEngine.UI;

public class ScrollViewController : MonoBehaviour
{
    public LoadSerie loadSerie; // R�f�rence pour charger les textures
    public RawImage[] cartes;   // Cartes dans le ScrollView (0: Pr�c�dente, 1: Actuelle, 2: Suivante)
    public ScrollRect scrollRect; // R�f�rence � la ScrollView

    private bool isScrolling = false; // Emp�che les mises � jour multiples

    public float timer;

    void Start()
    {
        scrollRect.onValueChanged.AddListener(OnScroll);
    }

    public void UpdateCartes()
    {
        // Met � jour les textes des cartes selon les indices
        if (loadSerie.selectedCardIndex - 2 > 0 || loadSerie.selectedCardIndex == 2)
        {
            cartes[0].texture = loadSerie.rawImage[loadSerie.selectedCardIndex - 2].texture;
            Debug.Log("Cards 0");
        }
        else
        {
            cartes[0].texture = loadSerie.defaultTexture;
        }
        if (loadSerie.selectedCardIndex > 0)
        {
            cartes[1].texture = loadSerie.rawImage[loadSerie.selectedCardIndex - 1].texture;
            Debug.Log("Cards 1");
        }
        if (loadSerie.selectedCardIndex < loadSerie.appManager.Nombre_de_carte)
        {
            cartes[2].texture = loadSerie.rawImage[loadSerie.selectedCardIndex].texture;
            Debug.Log("Cards 2");
        }
        else
        {
            cartes[2].texture = loadSerie.defaultTexture;
        }
        loadSerie.apiManager.LoadCard_Price();
        loadSerie.Updatecardssettings();
        loadSerie.UpdateToggle();
        ResetScroll();
    }

    public void OnScroll(Vector2 scrollPosition)
    {
        if (isScrolling) return;
        {
            timer += Time.deltaTime;
            if (scrollPosition.x <= 0.1 && timer > 0.4) // D�filement vers la gauche
            {
                timer = 0;
                isScrolling = true;
                MoveRight(); // Passe � la carte pr�c�dente
            }
            else if (scrollPosition.x >= 0.9 && timer > 0.4) // D�filement vers la droite
            {
                timer = 0;
                isScrolling = true;
                MoveLeft(); // Passe � la carte suivante
            }
        }
    }

    void MoveRight()
    {
        if (loadSerie.selectedCardIndex > 1)
        {
            loadSerie.selectedCardIndex -= 1;
            //Debug.Log("-1");
        }
        UpdateCartes();
    }

    void MoveLeft()
    {
        if(loadSerie.selectedCardIndex < loadSerie.appManager.Nombre_de_carte)
        {
            loadSerie.selectedCardIndex += 1;
            //Debug.Log("+1");
        }
        UpdateCartes();
    }
    void ResetScroll()
    {
        isScrolling = false;
        scrollRect.normalizedPosition = new Vector2(0.5f, 0);

    }
}
