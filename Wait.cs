using ExitGames.Client.Photon;
using System.Collections.Generic;
using UnityEngine;

public static class Wait
{
    public static Dictionary<int, WaitForSeconds> _caches = new Dictionary<int, WaitForSeconds>();

    public static WaitForSeconds WaitForSeconds(float k)
    {
        if (k > 999 || k < .01)
        {
            Debug.Log("<color=yellow><b>seconds is out of key range, return uncached WaitForSeconds</b></color>");
            return new WaitForSeconds(k);
        }

        int key = GetKey(k);

        if(!_caches.ContainsKey(key))
        {
            _caches.Add(key, new WaitForSeconds(k));
        }

        return _caches[key];
    }

    static int GetKey(float k) =>
        (int)k * 100;
}
