using UnityEngine;
using HtmlAgilityPack;
using System.Threading.Tasks;
using System;

public class ApiManager : MonoBehaviour
{
    public LoadSerie loadSerie;
    public string Card_Price;
    public async void LoadCard_Price()
    {
        loadSerie.Card_Price.text = "Loading...";
        string url = "";
        if (loadSerie.appManager.promo == true)
        {
            if(loadSerie.selectedCardIndex < 10)
            {
                url = loadSerie.appManager.api_url_debut +"0"+loadSerie.selectedCardIndex.ToString();
                Debug.Log("plus petite que 10");
            }else
            {
                if(loadSerie.selectedCardIndex > 9)
                {
                    url = loadSerie.appManager.api_url_debut + loadSerie.selectedCardIndex.ToString();
                    Debug.Log("plus grand que 9");
                }
            }
        }else
        {
            url = loadSerie.appManager.api_url_debut + loadSerie.selectedCardIndex.ToString();
        }
        Card_Price = await GetPokemonCardInfo(url);
        loadSerie.Card_Price.text = Card_Price;
    }

    private async Task<string> GetPokemonCardInfo(string url)
    {
        // Utiliser HttpClient pour télécharger le HTML
        using (var httpClient = new System.Net.Http.HttpClient())
        {
            var html = await httpClient.GetStringAsync(url);
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            // Utiliser HtmlAgilityPack pour analyser le HTML
            var cardPriceNode = htmlDoc.DocumentNode.SelectSingleNode("//p[@class='title is-size-6-mobile is-5 price-mid has-text-info']");

            string cardPrice = cardPriceNode != null ? cardPriceNode.InnerText : "Unknown";

            return $"Prix: {cardPrice}";
        }
    }
}