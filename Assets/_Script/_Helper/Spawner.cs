using UnityEngine;

public class Spawner : MonoBehaviour {



    ///////////////////////////////////////////////////////////////////////////////
    // funcitons
    ///////////////////////////////////////////////////////////////////////////////

    public void Init () {
        // indicator
        indicatorPool.Init();
        for ( int i = 0; i < indicatorPool.initData.Length; ++i ) {
            GameObject indicatorGO = indicatorPool.initData[i];
            Indicator indicator = indicatorGO.GetComponent<Indicator>();
            indicator.OnDespawned();
            GameObject.DontDestroyOnLoad(indicatorGO);
        }

        // cast indicator
        castIndicatorPool.Init();
        for ( int i = 0; i < castIndicatorPool.initData.Length; ++i ) {
            GameObject castIndicatorGO = castIndicatorPool.initData[i];
            CastIndicator castIndicator = castIndicatorGO.GetComponent<CastIndicator>();
            castIndicator.OnDespawned();
            GameObject.DontDestroyOnLoad(castIndicatorGO);
        }

        // targetLock 
        targetLockPool.Init();
        foreach (GameObject item in targetLockPool.initData)
        {
            TargetLock targetlock = item.GetComponent<TargetLock>();
            targetlock.OnDespawned();
            GameObject.DontDestroyOnLoad(item);
        }

        // currency text
        currencyTextPool.Init();
        for ( int i = 0; i < currencyTextPool.initData.Length; ++i ) {
            GameObject go = currencyTextPool.initData[i];
            CurrencyText comp = go.GetComponent<CurrencyText>();
            comp.OnDespawned();
            GameObject.DontDestroyOnLoad(go);
        }
    }

    public void Reset () {
    }
}

