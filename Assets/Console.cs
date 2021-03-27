using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Console : MonoBehaviour
{
    public GameObject input;
    public List<bool> toDesActives;
    public List<MonoBehaviour> toDes;
    public List<string> replics;
    public int replic;
    public GameObject text;
    public GameObject textholder;
    Config config;
    private void Start()
    {
        config = FindObjectOfType<WorldConfig>().config;
        Instantiate(text, textholder.transform).GetComponent<Text>().text = config.eng ? "$help - for help." : "$help - для помощи.";
    }

    private void Update()
    {
        if (input.active == false)
        {
            if (Input.GetKeyDown("`"))
            {
                textholder.GetComponent<VerticalLayoutGroup>().spacing = Random.Range(1, 1.125f);
                toDesActives.Clear();
                for (int i = 0; i < toDes.Count; i++)
                {
                    toDesActives.Add(toDes[i].enabled);
                    toDes[i].enabled = false;
                }
                input.SetActive(true);
            }
        }
        if (input.active == true)
        {
            var field = input.GetComponentInChildren<TMP_InputField>();
            bool phrase = false;
            for (int i = 0; i < replics.Count; i++)
            {
                if (field.text.ToLower() == replics[i].ToLower())
                {
                    replic = i;
                    phrase = true;
                    break;
                }
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (phrase == false)
                {
                    if (replics.Count > 0)
                    {
                        replic = 0;
                        field.text = replics[replic];
                    }
                }
                else
                {
                    if (replic + 1 <= replics.Count - 1)
                    {
                        replic++;
                        field.text = replics[replic];
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (phrase == false)
                {
                    if (replic < replics.Count-1)
                    {
                        replic = replics.Count - 1;
                        field.text = replics[replic];
                    }
                }
                else
                {
                    if (replic - 1 >= 0)
                    {
                        replic--;
                        field.text = replics[replic];
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (field.text.Replace(" ","") != "") {
                    replics.Add(field.text);
                    Instantiate(text, textholder.transform).GetComponent<Text>().text ="<color='fff'>" + GetComponent<PlayerStats>().localPlayer.name + "</color>: " +  field.text;
                }
                ProcessCommands(field);
                field.text = "";
                field.Select();
            }
        }
    }
    public void Close()
    {
        for (int i = 0; i < toDes.Count; i++)
        {
            toDes[i].enabled = toDesActives[i];
        }
        input.SetActive(false);
    }

    public void ProcessCommands(TMP_InputField field)
    {
        if (field.text.Length > 0)
        {
            if (field.text[0] == '$')
            {
                field.text = field.text.ToLower();
                string[] command = field.text.Split(' ');
                if (command[0].Contains("$gi"))
                {
                    if (command.Length == 3)
                    {
                        string item = command[1];
                        int number;
                        bool success = int.TryParse(command[2], out number);
                        if (success)
                        {
                            var it = FindObjectOfType<WorldManager>().GetItemBySecondName(item, true);
                            if (it != null)
                            {
                                it.value = number;
                                GetComponent<PlayerUI>().AddItem(it);
                            }
                        }
                        print(command[1] + "|" + number + "|" + success);
                    }
                }
                if (command[0].Contains("$clear"))
                {
                    foreach (Transform item in textholder.transform)
                    {
                        Destroy(item.gameObject);
                    }
                }
                if (command[0].Contains("$ilist"))
                {
                    var it = FindObjectOfType<WorldManager>();
                    Instantiate(text, textholder.transform).GetComponent<Text>().text = "Список вещей:";
                    for (int i = 0; i < it.items.Count; i++)
                    {
                        if (command.Length == 1)
                        {
                            Instantiate(text, textholder.transform).GetComponent<Text>().text =
                            "#" + i + " - " + it.items[i].name + " - [id: " + it.items[i].secondName + "]";
                        }
                        if (command.Length == 2)
                        {
                            if (it.items[i].name.ToLower().Contains(command[1].ToLower()) || it.items[i].secondName.ToLower().Contains(command[1].ToLower()))
                            {
                                Instantiate(text, textholder.transform).GetComponent<Text>().text =
                                "#" + i + " - " + it.items[i].name + " - [id: " + it.items[i].secondName + "]";
                            }
                        }
                    }
                    Instantiate(text, textholder.transform).GetComponent<UnityEngine.UI.Text>().text = "";
                }
                if (command[0].Contains("$god+"))
                {
                    GetComponent<PlayerStats>().localPlayer.canDeath = false;
                    Instantiate(text, textholder.transform).GetComponent<Text>().text = "God mod on";
                }
                if (command[0].Contains("$god-"))
                {
                    GetComponent<PlayerStats>().localPlayer.canDeath = true;
                    Instantiate(text, textholder.transform).GetComponent<Text>().text = "God mod off";
                }
                if (command[0].Contains("$spawn"))
                {
                    transform.position = new Vector2(GetComponent<PlayerStart>().playerStart.x, GetComponent<PlayerStart>().playerStart.y + 2);
                }
                if (command[0].Contains("$gopos"))
                {
                    if (command.Length >= 2)
                    {
                        if (command[1] != "curr" && command[2] != "curr")
                        {
                            transform.position = new Vector2(float.Parse(command[1]), float.Parse(command[2]));
                            Instantiate(text, textholder.transform).GetComponent<Text>().text = "Pos: " + transform.position.x + " " + transform.position.y;
                        }
                        if (command[1] == "curr" && command[2] != "curr")
                        {
                            transform.position = new Vector2(transform.position.x, float.Parse(command[2]));
                            Instantiate(text, textholder.transform).GetComponent<Text>().text = "Pos: curr " + transform.position.y;
                        }
                        if (command[1] != "curr" && command[2] == "curr")
                        {
                            transform.position = new Vector2(float.Parse(command[1]), transform.position.y);
                            Instantiate(text, textholder.transform).GetComponent<Text>().text = "Pos: " + transform.position.x + " curr";
                        }
                        if (command[1] == "curr" && command[2] == "curr")
                        {
                            transform.position = transform.position + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1));
                            Instantiate(text, textholder.transform).GetComponent<Text>().text = "Pos: curr curr";
                        }
                    }
                }
                if (command[0].Contains("$getpos"))
                {
                    Instantiate(text, textholder.transform).GetComponent<Text>().text = "Pos: " + ((Vector2)transform.position).ToString();
                }
                if (command[0].Contains("$hideui"))
                {
                    Close();
                    GetComponent<PlayerUI>().mainCanvas.SetActive(!GetComponent<PlayerUI>().mainCanvas.active);
                }
                if (command[0].Contains("$full"))
                {
                    Instantiate(text, textholder.transform).GetComponent<Text>().text = "Pos fulled.";
                    GetComponent<PlayerStats>().localPlayer.health = GetComponent<PlayerStats>().localPlayer.stdHealth + GetComponent<PlayerStats>().localPlayer.dopHealth;
                }
                if (command[0].Contains("$language"))
                {
                    if (command[1] == "ru" || command[1] == "rus" || command[1] == "russian" || command[1] == "ру" || command[1] == "русский")
                    {
                        FindObjectOfType<WorldConfig>().config.eng = false;
                        config = FindObjectOfType<WorldConfig>().config;
                        Instantiate(text, textholder.transform).GetComponent<Text>().text = "Язык изменён на русский";
                    }
                    if (command[1] == "en" || command[1] == "english")
                    {
                        FindObjectOfType<WorldConfig>().config.eng = true;
                        config = FindObjectOfType<WorldConfig>().config;
                        Instantiate(text, textholder.transform).GetComponent<Text>().text = "Language changed to English";
                    }
                }
                if (command[0].Contains("$help"))
                {
                    config = FindObjectOfType<WorldConfig>().config;
                    Instantiate(text, textholder.transform).GetComponent<Text>().text = "$gi _itemid_ count - " + (config.eng ? "creates an item." : "создаёт предмет.");
                    Instantiate(text, textholder.transform).GetComponent<Text>().text = "$clear - " + (config.eng ? "clears text screen." : "очищает текстовый экран.") ;
                    Instantiate(text, textholder.transform).GetComponent<Text>().text = "$ilist - " + (config.eng ? "returns a list of items with id." : "возвращает список предметов с id.");
                    Instantiate(text, textholder.transform).GetComponent<Text>().text = "$ilist str - " + (config.eng ? "returns a list of items in id or whose name contains str." : "возвращает список предметов в id или имени которых содержится str.");
                    Instantiate(text, textholder.transform).GetComponent<Text>().text = "$god-/+ - " + (config.eng ? "become immortal or not." : "стать бессмертным или нет.");
                    Instantiate(text, textholder.transform).GetComponent<Text>().text = "$spawn - " + (config.eng ? "teleport to spawn." : "телепорт на спавн.");
                    Instantiate(text, textholder.transform).GetComponent<Text>().text = "$goPos X Y - " + (config.eng ? "teleport to X Y (curr - position now)." : "телепорт на X Y (curr - позиция сейчас).");
                    Instantiate(text, textholder.transform).GetComponent<Text>().text = "$getPos - " + (config.eng ? "get your coordinates." : "получить ваши координаты.");
                    Instantiate(text, textholder.transform).GetComponent<Text>().text = "$hideui - " + (config.eng ? "hide UI." : "скрыть UI.");
                    Instantiate(text, textholder.transform).GetComponent<Text>().text = "$full - " + (config.eng ? "replenish health." : "пополнить здоровье.");
                    Instantiate(text, textholder.transform).GetComponent<Text>().text = "$language - [ru, en]" + (config.eng ? "switch language." : "смена языка.");
                }
            }
        }
    }
}
