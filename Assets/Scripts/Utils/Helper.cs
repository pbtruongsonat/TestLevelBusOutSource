using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public static partial class Helper
{
    public static void SetActiveAll<T>(this IList<T> list, bool value) where T : Component
    {
        foreach (var x1 in list)
            x1.gameObject.SetActive(value);
    }

    public static void SetActiveAll(this IList<GameObject> list, bool value)
    {
        foreach (var x1 in list)
            x1.SetActive(value);
    }

    public static void OnChanged<T>(this IEnumerable<T> scripts, bool value) where T : ToggleScript
    {
        foreach (var toggleScript in scripts)
            if (toggleScript == null)
                Debug.LogError("ToggleScript is null");
            else
                toggleScript.OnChanged(value);
    }

    public static void LoadData<T>(this IEnumerable<T> scripts) where T : BoosterBase
    {
        foreach (var booster in scripts)
            if (booster == null)
                Debug.LogError("BoosterBase is null");
            else
                booster.LoadData();
    }

    public static T GetRandomElementInList<T>(this List<T> list)
    {
        int index = UnityEngine.Random.Range(0, list.Count);
        return list[index];
    }

    public static void SetLocalPosZ(this Transform transform, float z)
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, z);
    }

    public static void SetPosY(this Transform transform, float y)
    {
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }

    public static void SetPosZ(this Transform transform, float z)
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, z);
    }

    public static void SetScaleX(this Transform transform, float x)
    {
        transform.localScale = new Vector3(x, transform.localScale.y, transform.localScale.z);
    }

    public static DateTime StringToDate(this string date)
    {
        if (string.IsNullOrEmpty(date))
        {
            return DateTime.MinValue;
        }

        try
        {
            return DateTime.ParseExact(date, "MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);

        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        return DateTime.Now;
    }

    public static string DateTimeToString(this DateTime dateTime)
    {
        return dateTime.ToString("MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
    }

    public static bool IsPurchaserInitFailed()
    {
        return false;
    }
}

[Serializable]
public class Vector2Serialized
{
    public float x, y;

    public Vector2Serialized(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public Vector2Serialized()
    {

    }

    public Vector2 Value()
    {
        return new Vector2(x, y);
    }
}
