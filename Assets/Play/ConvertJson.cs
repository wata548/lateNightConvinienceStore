using System;
using System.Text;
using UnityEngine.UI;
using UnityEngine.Windows;
using File = System.IO.File;
using Image = Microsoft.Unity.VisualStudio.Editor.Image;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class MatchItemAndCount {

    private string name;
    private string number;
}

[Serializable]
public class CharacterRaw {
    
    public string name;
    public string defaultImagePath;
    public string[] image;
    public MatchItemAndCount[] buyingItem1;
    public MatchItemAndCount[] buyingItem2;
    public MatchItemAndCount[] buyingItem3;
}
public class Character {
    private string name;
    private Dictionary<string, Sprite> images;
    private MatchItemAndCount[] purchaseList1;
    private MatchItemAndCount[] purchaseList2;
    private MatchItemAndCount[] purchaseList3;

    public Character(CharacterRaw rawInfo) {

        name = rawInfo.name;

        images = new();
        foreach (var imageName in rawInfo.image) {

            var image = Resources.Load<Sprite>(rawInfo.defaultImagePath + imageName);
            images.Add(imageName, image);
        }

        purchaseList1 = rawInfo.buyingItem1;
        purchaseList2 = rawInfo.buyingItem2;
        purchaseList3 = rawInfo.buyingItem3;
    }
}

public class Item {
    public string name;
    public int price;
}

public class ConvertJson: MonoBehaviour {

    private Dictionary<string, int> itemInfo = new();
    private Character[] characterInfo;
    private string peopleInfoPath = "Assets\\Resources\\Jsons\\People.json";
    private string itemInfoPath = "Assets\\Resources\\Jsons\\itemInfo.json";
    public void Decode() {

        string json = File.ReadAllText(peopleInfoPath);
        var characterRawInfo = JsonConvert.DeserializeObject<CharacterRaw[]>(json);
        characterInfo = characterRawInfo
            .Select((rawInfo) => new Character(rawInfo))
            .ToArray();
        
        json = File.ReadAllText(itemInfoPath);
        var items = JsonConvert.DeserializeObject<Item[]>(json);
        foreach(var item in items) {
            itemInfo.Add(item.name, item.price);
        }
    }

    private void Awake() {
        Decode();
    }
}