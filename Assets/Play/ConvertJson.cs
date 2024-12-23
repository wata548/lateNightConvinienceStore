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
public class MatchItemAndCountRaw {

    public string  name;
    public int     number;
    
}
[Serializable]
public class CharacterRaw {
    
    public string name;
    public string defaultImagePath;
    public string[] image;
    public MatchItemAndCountRaw[] buyingItem1;
    public MatchItemAndCountRaw[] buyingItem2;
    public MatchItemAndCountRaw[] buyingItem3;
}

public class MatchItemAndCount {
    
    public string Name { get; private set; }
    public int Number { get; private set; }

    public MatchItemAndCount(string name, int number) {
        Name = name;
        Number = number;
    }
    public static explicit operator MatchItemAndCount(MatchItemAndCountRaw target) {

        var casting = new MatchItemAndCount(
            target.name,
            target.number
        );

        return casting;
    }

    public override string ToString() {
        return $"{Name}, {Number}개";
    }
}
public class Character {
    public string Name { get; private set; }
    public Dictionary<string, Sprite> Images { get; private set; }
    public MatchItemAndCount[] PurchaseList1 { get; private set; }
    public MatchItemAndCount[] PurchaseList2 { get; private set; }
    public MatchItemAndCount[] PurchaseList3 { get; private set; }

    public Character(CharacterRaw rawInfo) {

        Name = rawInfo.name;

        Images = new();
        foreach (var imageName in rawInfo.image) {

            var image = Resources.Load<Sprite>(rawInfo.defaultImagePath + imageName);
            Images.Add(imageName, image);
        }

        PurchaseList1 = rawInfo.buyingItem1
            .Select((info) => (MatchItemAndCount)info)
            .ToArray();
        PurchaseList2 = rawInfo.buyingItem2
            .Select((info) => (MatchItemAndCount)info)
            .ToArray();
        PurchaseList3 = rawInfo.buyingItem3
            .Select((info) => (MatchItemAndCount)info)
            .ToArray();
    }
}

[Serializable]
public class ItemRaw {
    public string item;
    public int price;
    public string image;
}

public class ConvertJson: MonoBehaviour {

    public static ConvertJson Instance { get; private set; } = null;
    
    private readonly Dictionary<string, int> itemInfo = new();
    private Dictionary<string, Character> characterInfo;
    private readonly string peopleInfoPath = "Assets\\Resources\\Jsons\\People.json";
    private readonly string itemInfoPath = "Assets\\Resources\\Jsons\\itemInfo.json";
    public List<string> PeopleList { get; private set; }= null;
    
    public void Decode() { string json = File.ReadAllText(peopleInfoPath); var characterRawInfo = JsonConvert.DeserializeObject<CharacterRaw[]>(json);
        characterInfo = characterRawInfo
            .Select((rawInfo) => new Character(rawInfo))
            .ToDictionary((character => character.Name));

        PeopleList = characterRawInfo
            .Select(rawInfo => rawInfo.name)
            .ToList();
        
        json = File.ReadAllText(itemInfoPath);
        var items = JsonConvert.DeserializeObject<ItemRaw[]>(json);
        foreach(var item in items) {
            itemInfo.Add(item.item, item.price);
        }
    }

    public Character GetCharacter(string name) {
        if (!characterInfo.TryGetValue(name, out Character character))
            throw new Exception($"This name is not correct: {name}");
        
        return character;
    }

    public int GetPrice(string name) {
        if (!itemInfo.TryGetValue(name, out int price))
            throw new Exception($"This name is not correct: {name}");

        return price;
    }

    private void Awake() {
        
        Decode();
        Instance ??= this;
    }
}