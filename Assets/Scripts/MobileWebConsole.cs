using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MobileConsoleMessage
{
	public string log;
	
	public string stack;
	
	public LogType type;
	
	public string time;
}

public class MobileWebConsole : MonoBehaviour
{
	static MobileWebConsole instance;
	
	static List<MobileConsoleMessage> log = new List<MobileConsoleMessage>();
	
	void Awake()
	{
		DontDestroyOnLoad(gameObject);
		
		Application.RegisterLogCallback (Print);
	}
	
	public static void Init()
	{
		if(instance == null)
		{
			GameObject go = new GameObject("MobileWebConsole");
			
			instance = go.AddComponent<MobileWebConsole>();
		}
	}
	
	public static void Print(string logString, string stackTrace, LogType logType)
	{
		//if (logType != LogType.Log)
		{
			log.Add(new MobileConsoleMessage(){log = logString, stack = stackTrace, type = logType, time = System.DateTime.Now.ToString("hh:mm:ss")});
		}
	}
	
	public static void Print(object message)
	{
		Print(message.ToString(), "", LogType.Log);
	}
	
	bool isHided = true;
	float scrollPosition = 0;
	
	MobileConsoleMessage expandedMessage;
	
	float logWidth = 400;
	
	float showButtonWidth = 70;
	bool showDebug = false;
	
	void OnGUI()
	{		
		showDebug = GUI.Toggle(new Rect(Screen.width - 20, 5, 15, 15), showDebug, "");
		if (!showDebug)
		{
			return;
		}
		
		int count = isHided ? Mathf.Min(1, log.Count) : Mathf.Min(10, log.Count);
		
		Rect consoleRect = new Rect(0, Screen.height - count * 40, logWidth + 40 + showButtonWidth, count * 40);
		
		GUI.Box(consoleRect, "");
		for(int i = 0; i < count; i++)
		{
			MobileConsoleMessage message = log[log.Count - 1 - i - Mathf.FloorToInt(scrollPosition)];
			
			if(message.type == LogType.Error || message.type == LogType.Exception)
			{
				GUI.color = Color.red;
			}
			else if(message.type == LogType.Warning)
			{
				GUI.color = Color.yellow;
			}
			else
			{
				GUI.color = Color.white;
			}
			
			GUI.TextField(new Rect(0, Screen.height - count * 40 + i * 40, 70, 40), message.time);
			GUI.TextField(new Rect(70, Screen.height - count * 40 + i * 40, logWidth - 70, 40), message.log);
			
			//			if(GUI.Button(new Rect(logWidth, Screen.height - count * 20 + i * 20, 20, 20), "X"))
			//			{
			//				if(message == expandedMessage)
			//				{
			//					expandedMessage = null;
			//				}
			//				log.RemoveAt(log.Count - 1 - i);
			//				
			//				break;
			//			}
			if(GUI.Button(new Rect(logWidth, Screen.height - count * 40 + i * 40, 40, 40), ">"))
			{
				expandedMessage = message;
			}
		}
		
		GUI.color = Color.white;
		
		if(GUI.Button(new Rect(logWidth + 40, Screen.height - count * 40, showButtonWidth, 40), isHided ? ("Show" + (((log.Count - 1) == 0) ? "" : "(" + (log.Count - 1) + ")")) : "Hide"))
		{
			isHided = !isHided;
		}
		
		if(GUI.Button(new Rect(logWidth + 40, Screen.height - count * 40 + 40, showButtonWidth, 40), "Clear"))
		{
			log.Clear();
			
			expandedMessage = null;
			
			isHided = true;
			
			scrollPosition = 0;
			
			return;
		}
		
		if(!isHided)
		{
			if(log.Count > count)
			{
				scrollPosition = GUI.VerticalScrollbar(new Rect(logWidth + 40, Screen.height - count * 40 + 80, 40, count * 40 - 40), scrollPosition, log.Count - 1 - (log.Count - count), 0, log.Count - 1);
			}
			else
			{
				scrollPosition = 0;
			}
		}
		else
		{
			scrollPosition = 0;	
		}
		
		if(expandedMessage != null)
		{
			if(expandedMessage.type == LogType.Error || expandedMessage.type == LogType.Exception)
			{
				GUI.color = Color.red;
			}
			else if(expandedMessage.type == LogType.Warning)
			{
				GUI.color = Color.yellow;
			}
			else
			{
				GUI.color = Color.white;
			}
			
			Rect stackRect = new Rect(logWidth + 40 + showButtonWidth, Screen.height - 200, 370, 200);
			
			GUI.Box(stackRect, "");
			
			GUI.TextArea(new Rect(logWidth + 40 + showButtonWidth, Screen.height - 200, 450 - 80, 200), expandedMessage.log + "(" + expandedMessage.time + ")" + "\n" + expandedMessage.stack);
			
			if(GUI.Button(new Rect(logWidth + 40 + showButtonWidth + 370 - 40, Screen.height - 240, 40, 40), "<"))
			{
				expandedMessage = null;
			}
		}
		
		GUI.color = Color.white;
	}
}
