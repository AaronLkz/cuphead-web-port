using System;
using System.Collections.Generic;
using UnityEngine;

public class ComponentReferenceManager
{
	public static ComponentReferenceManager Instance = new ComponentReferenceManager();

	private Dictionary<string, WeakReference> refs = new Dictionary<string, WeakReference>();

	private int count;

	public void AddRef(Component c)
	{
		string key = string.Format("Index:<color=red>{0}</color> ComponentType:<color=red>{1}</color> GameObject:<color=red>{2}</color>", count, c.GetType().ToString(), GetGameObjectPath(c));
		refs[key] = new WeakReference(c);
		count++;
	}

	private string GetGameObjectPath(Component c)
	{
		GameObject gameObject = c.gameObject;
		string text = "/" + gameObject.name;
		while (gameObject.transform.parent != null)
		{
			gameObject = gameObject.transform.parent.gameObject;
			text = "/" + gameObject.name + text;
		}
		return text;
	}

	public void PrintLog()
	{
		GC.Collect();
		Debug.LogError("Objects that are destroyed but still referenced:");
		foreach (KeyValuePair<string, WeakReference> @ref in refs)
		{
			if (!@ref.Value.IsAlive)
			{
				continue;
			}
			if (@ref.Value.Target == null)
			{
				Debug.LogErrorFormat("Target is null {0}", @ref.Key);
				continue;
			}
			Component component = @ref.Value.Target as Component;
			if (component == null)
			{
				Debug.LogErrorFormat("Component is null {0}", @ref.Key);
			}
			else if (component.gameObject == null)
			{
				Debug.LogErrorFormat("Component attached game object is null {0}", @ref.Key);
			}
		}
	}
}
