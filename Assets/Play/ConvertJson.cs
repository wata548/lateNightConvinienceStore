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
using NUnit.Framework.Constraints;
using UnityEngine.Serialization;

[Serializable]
public class MatchItemAndCountRaw {

    public string  name;
    public int     number;
    public int     eventCount = 0;
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
    public int Event { get; private set; }

    public MatchItemAndCount(string name, int number, int eventCount) {
        Name = name;
        Number = number;
        Event = eventCount;
    }
    public static explicit operator MatchItemAndCount(MatchItemAndCountRaw target) {

        var casting = new MatchItemAndCount(
            target.name,
            target.number,
            target.eventCount
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

[Serializable]
public class DialogRaw {

    public string situation;
    public string[] actors;
    public DialogDetailRaw[] scripts;
}

[Serializable]
public class DialogsRaw {

    public string speaker;
    public DialogRaw[] communications;
}

[Serializable]
public class DialogDetailRaw {

    public int actor;
    public string script;
    public bool skip = false;
}

public class Dialog {
    public string Situation { get; private set; }
    public List<string> Actors { get; private set; }
    public List<DialogDetail> Scripts { get; private set; }

    public Dialog(string situation, string[] actors, DialogDetailRaw[] scripts) {

        Situation = situation;
        Actors = actors.ToList();
        Scripts = scripts
            .Select(rawData => (DialogDetail)rawData)
            .ToList();
    }

    public static explicit operator Dialog(DialogRaw target) {

        var result = new Dialog(target.situation, target.actors, target.scripts);
        return result;
    }
}

public class DialogDetail {
    public int Actor { get; private set; }
    public string Script { get; private set; }
    public bool Skip { get; private set; }

    public DialogDetail(int actor, string script, bool skip = false) {

        Actor = actor;
        Script = script;
        Skip = skip;
    }

    public static explicit operator DialogDetail(DialogDetailRaw target) {

        var result = new DialogDetail(target.actor, target.script, target.skip);
        return result;
    }
}

public class Dialogs {
    
    public string Speaker { get; private set; }
    public Dictionary<string, Dialog> Communications { get; private set; }

    public Dialogs(string speaker, DialogRaw[] communications) {

        Speaker = speaker;
        Communications = communications
            .Select(Communications => (Dialog)Communications)
            .ToDictionary(dialog => dialog.Situation);
    }
    public static explicit operator Dialogs(DialogsRaw target) {

        Dialogs result = new Dialogs(target.speaker, target.communications);
        return result;
    }
}

public class ConvertJson: MonoBehaviour {

    public static ConvertJson Instance { get; private set; } = null;
    
   //==================================================||Field 
    private readonly Dictionary<string, int> itemInfo = new();
    private Dictionary<string, Character> characterInfo;
    private Dictionary<string, Dialogs> CommunicationInfo;
    private readonly string peopleInfoPath = "Assets\\Resources\\Jsons\\People.json";
    private readonly string itemInfoPath = "Assets\\Resources\\Jsons\\itemInfo.json";
    private readonly string communicationInfoPath = "Assets\\Resources\\Jsons\\communications.json";
    public List<string> PeopleList { get; private set; }= null;
    
   //==================================================||Method 
    public void Decode() { 
        string json = File.ReadAllText(peopleInfoPath);
        var characterRawInfo = JsonConvert.DeserializeObject<CharacterRaw[]>(json);
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

        json = File.ReadAllText(communicationInfoPath);
        var communications = JsonConvert.DeserializeObject<DialogsRaw[]>(json);
        CommunicationInfo = communications
            .Select((raw => (Dialogs)raw))
            .ToDictionary(datas => datas.Speaker);
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

    public Dialog GetDialog(string speaker, string situation) {

        if (!CommunicationInfo.TryGetValue(speaker, out Dialogs dialogs))
            throw new IndexOutOfRangeException($"speaker: {speaker} didn't exist");

        if (!dialogs.Communications.TryGetValue(situation, out Dialog situationDialog))
            throw new IndexOutOfRangeException($"situation: {speaker}-{situation} didn't exsit");

        return situationDialog;
    }

    public string GetActor(string speaker, string situation, int index) {

        var situationDialog = GetDialog(speaker, situation);
        if (situationDialog.Actors.Count <= index)
            throw new IndexOutOfRangeException($"index: {index} is over than {situationDialog.Actors.Count}");

        int actor = situationDialog.Scripts[index].Actor;
        return situationDialog.Actors[actor];
    }

    public bool GetSkip(string speaker, string situation, int index) {
        
        var situationDialog = GetDialog(speaker, situation);
        if (situationDialog.Actors.Count <= index)
            throw new IndexOutOfRangeException($"index: {index} is over than {situationDialog.Actors.Count}");
        
        return situationDialog.Scripts[index].Skip;
    }

    public string GetScript(string speaker, string situation, int index) {
        
        var situationDialog = GetDialog(speaker, situation);
        if (situationDialog.Actors.Count <= index)
            throw new IndexOutOfRangeException($"index: {index} is over than {situationDialog.Actors.Count}");
                
        return situationDialog.Scripts[index].Script;
    }

   //==================================================||Unity Logic 
    private void Awake() {
        
        Decode();
        Instance ??= this;
    }
}