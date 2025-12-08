using System;
using System.Collections;
using System.Collections.Generic;
using SUSDK.Entity;
using SUSDK.Manager;
using SUSDK.Util;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace SUSDK.Console
{
  public class DebugController : MonoBehaviour
  {
    private static DebugCommand _help;
    private static DebugCommand<string> _god;
    private static DebugCommand<string, int> _setHealth;
    private static DebugCommand<int, int> _spawn;
    private static DebugCommand _reloadScene;
    private static DebugCommand _clear;
    private static DebugCommand<bool> _debug;
    [HideInInspector]
    public bool showConsole;
    private List<object> _commandList;
    private List<string> _commandLog;
    private string _input;
    private Vector2 _scroll;
    private bool _showMenu;
    private bool _debugLogEnabled;
    [HideInInspector]
    public bool hideCursorOnClose;

    private void HandleUnityLog(string logString, string stackTrace, LogType type)
    {
      if (type == LogType.Log)
      {
        _commandLog.Add($"[{type}] {logString}"); 
      }
    }
    
    private void Awake()
    {
      gameObject.AddComponent<CoroutineStarter>();
      if(hideCursorOnClose)
      {
        Cursor.visible = false; 
      }
      
      AddDefaultCommands();
    }
    
    public void AddCommand(string command, string description, string format, Action action)
    {
      var newCommand = new DebugCommand(command, description, format, action);
      _commandList.Add(newCommand);
    }
    
    public void RemoveCommand(string command)
    {
      _commandList.RemoveAll(x => (x as DebugCommandBase)!.CommandId.Equals(command));
    }

    private void AddDefaultCommands()
    {
       _help = new DebugCommand("help", "Shows a list of commands", "help", () =>
      {
        for (var i = 0; i < _commandList.Count; i++)
        {
          var command = _commandList[i] as DebugCommandBase;
          var label = $"[Info] {command?.CommandFormat} - {command?.CommandDescription}";
          _commandLog.Add(label);
        }
      });
      
      _debug = new DebugCommand<bool>(
        "debug",
        "Toggles redirecting Unity logs into the custom command log",
        "debug <true|false>",  (state) =>
        {
          _debugLogEnabled = state;
          if (_debugLogEnabled)
          {
            Application.logMessageReceived += HandleUnityLog;
            _commandLog.Add("[Info] Debug log redirection ENABLED");
          }
          else
          {
            Application.logMessageReceived -= HandleUnityLog;
            _commandLog.Add("[Info] Debug log redirection DISABLED");
          }
        }
      );

      _god = new DebugCommand<string>("god", "Disables the ability to take damage for a GameEntity", "god <entityName>", x =>
      {
        var entity = GameObject.Find(x);
        if (entity)
        {
          var gameEntity = entity.GetComponent<GameEntity>();
          if (gameEntity)
          {
            var currentStatus = gameEntity.canTakeDamage;
            gameEntity.canTakeDamage = !currentStatus;
            _commandLog.Add("[Warning] Entity '"+x+"' canTakeDamage = " + !currentStatus);
            return;
          }
          _commandLog.Add("[Warning] Entity does not have a GameEntity property");
        }
        else
        {
          _commandLog.Add("[Warning] No entity found under '"+x+"'");  
        }
      });

      _setHealth = new DebugCommand<string, int>("set_health", "Sets the hit points for a GameEntity", "set_health <entityName> <value>",
        (x,y) =>
        {
          var entity = GameObject.Find(x);
          if (entity)
          {
            var gameEntity = entity.GetComponent<GameEntity>();
            if (gameEntity)
            {
              gameEntity.hitPoints = y;
              _commandLog.Add("[Warning] Entity '"+x+"' hitPoints = " + y);
              return;
            }
            _commandLog.Add("[Warning] Entity does not have a GameEntity property");
          }
          else
          {
            _commandLog.Add("[Warning] No entity found under '"+x+"'");  
          }
        });

      _reloadScene = new DebugCommand("scene_reload", "Reloads the Scene and restarts the level", "scene_reload", () =>
      {
        _commandLog.Add("[Info] Reloading scene...");
        var before = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        _commandLog.Add("[Info] Reloaded Scene in " + (now - before) + "ms!");
      });

      _spawn = new DebugCommand<int, int>("spawn", "Spawns x times the given object", "spawn <id> <amount>", (x, y) =>
      {
        var items = ObjectManager.ObjectDictionary.Keys;
        var objectList = new List<string>(items);

        if (objectList.Count < x)
        {
          _commandLog.Add("[Warning] There is no object with the name '" + x + "'");
          _commandLog.Add("[Info] Printing object list...");
          var index = 1;
          foreach (var objectName in objectList)
          {
            _commandLog.Add("[Info] " + index + ": " + objectName);
            index++;
          }

          return;
        }

        var gameObjectToSpawn = ObjectManager.GetObject(objectList[x - 1]);
        _commandLog.Add("[Info] Spawning " + gameObjectToSpawn.name + " " + y + " times");

        var spawnPosition = gameObjectToSpawn.transform.position;

        if (SkipperoUnitySdk.Instance.ThreeDimensional)
        {
          var ray = Camera.main!.ScreenPointToRay(Input.mousePosition);
          if (Physics.Raycast(ray, out var hit))
          {
            spawnPosition = hit.point;
          }

        }
        else
        {
          var mouseWorldPos = Camera.main!.ScreenToWorldPoint(Input.mousePosition);
          mouseWorldPos.z = 0;
          spawnPosition = mouseWorldPos;
        }
        CoroutineStarter.Start(SpawnObjectsCoroutine(gameObjectToSpawn, y, spawnPosition));
      });
      
      _clear = new DebugCommand("clear", "clear console", "clear", () =>
      {
        _commandLog = new List<string>
        {
          "[System] Version " + Application.version,
          "[Info] Use 'help' for a list of commands",
        };
      });

      _commandList = new List<object>
      {
        _help,
        _reloadScene,
        _god,
        _setHealth,
        _spawn,
        _clear,
        _debug
      };

      _commandLog = new List<string>
      {
        "[System] Version " + Application.version,
        "[Info] Use 'help' for a list of commands",
      };
    }

    private void LateUpdate()
    {
      if (Input.GetKeyDown(KeyCode.Comma)) OnToggleDebug();

      if (Input.GetKeyDown(KeyCode.Return)) OnReturn();
    }

    private void OnEnable()
    {
      Application.logMessageReceived += Log;
    }

    private void OnDisable()
    {
      Application.logMessageReceived -= Log;
    }

    private void OnGUI()
    {
      if (!showConsole) return;

      var y = 0f;

      GUI.Box(new Rect(0, y, Screen.width, 300), "");

      var viewport = new Rect(0, 0, Screen.width - 30, 20 * _commandLog.Count);

      _scroll = GUI.BeginScrollView(new Rect(0, y + 5f, Screen.width, 290), _scroll, viewport);

      for (var i = 0; i < _commandLog.Count; i++)
      {
        var label = _commandLog[i];

        var labelRect = new Rect(5, 20 * i, viewport.width - 100, 20);

        GUI.Label(labelRect, label);
      }

      GUI.EndScrollView();

      y += 300;
      
      Cursor.visible = true;
      Time.timeScale = 0f;

      GUI.Box(new Rect(0, y, Screen.width, 30), "");
      GUI.backgroundColor = new Color(0, 0, 0, 0);
      _input = GUI.TextField(new Rect(10f, y + 5f, Screen.width - 20f, 20f), _input);

      var e = UnityEngine.Event.current;
      if (e.keyCode == KeyCode.Return) OnReturn();
    }

    public void Log(string logString, string stackTrace, LogType type)
    {
      if (type.Equals(LogType.Log)) return;
      _commandLog.Add("[" + type + "] " + logString);
      if (!string.IsNullOrEmpty(stackTrace)) _commandLog.Add("[Stacktrace] " + stackTrace);
    }

    public void OnToggleDebug()
    {
      showConsole = !showConsole;
      if (showConsole) return;
      if(hideCursorOnClose)
      {
        Cursor.visible = false; 
      }
      Time.timeScale = 1f;
    }

    public void OnToggleMenu()
    {
      if (SceneManager.GetActiveScene().name == "Intro" || SceneManager.GetActiveScene().name == "Menu") return;
      _showMenu = !_showMenu;
      if (_showMenu) return;
      if(hideCursorOnClose)
      {
        Cursor.visible = false; 
      }
      Time.timeScale = 1f;
    }

    public void OnReturn()
    {
      if (!showConsole) return;
      HandleInput();
      _input = "";
    }

    private static IEnumerator SpawnObjectsCoroutine(GameObject gameObjectToBeSpawned, int amount, Vector3 spawnPosition)
    {
      for (var i = 0; i < amount; i++)
      {
        Instantiate(gameObjectToBeSpawned, spawnPosition, Quaternion.identity);
        yield return null;
      }
    }

    private void HandleInput()
    {
      var properties = _input.Split(' ');

      for (var i = 0; i < _commandList.Count; i++)
      {
        var commandBase = _commandList[i] as DebugCommandBase;
        if (properties[0].ToLower().Equals(commandBase!.CommandId.ToLower()))
        {
          switch (_commandList[i])
          {
            case DebugCommand:
              (_commandList[i] as DebugCommand)!.Invoke();
              break;
            case DebugCommand<int> when properties.Length != 2:
              _commandLog.Add("[Error] Please enter a valid value");
              return;
            case DebugCommand<int>:
              (_commandList[i] as DebugCommand<int>)!.Invoke(int.Parse(properties[1]));
              break;
            default:
            {
              switch (_commandList[i])
              {
                case DebugCommand<bool> when properties.Length != 2:
                  _commandLog.Add("[Error] Please enter a valid value");
                  return;
                case DebugCommand<bool>:
                  (_commandList[i] as DebugCommand<bool>)!.Invoke(bool.Parse(properties[1]));
                  break;
                case DebugCommand<int, int> when properties.Length != 3:
                  _commandLog.Add("[Error] Please enter two valid values");
                  return;
                case DebugCommand<int, int>:
                  (_commandList[i] as DebugCommand<int, int>)!.Invoke(int.Parse(properties[1]), int.Parse(properties[2]));
                  break;
                case DebugCommand<string, int> when properties.Length != 3:
                  _commandLog.Add("[Error] Please enter two valid values");
                  return;
                case DebugCommand<string, int>:
                  (_commandList[i] as DebugCommand<string, int>)!.Invoke(properties[1], int.Parse(properties[2]));
                  break;
                case DebugCommand<string> when properties.Length != 2:
                  _commandLog.Add("[Error] Please enter a valid value");
                  return;
                case DebugCommand<string>:
                  (_commandList[i] as DebugCommand<string>)!.Invoke(properties[1]);
                  break;
              }
              break;
            }
          }
        }
        _scroll.y = int.MaxValue;
      }
    }
  }
}