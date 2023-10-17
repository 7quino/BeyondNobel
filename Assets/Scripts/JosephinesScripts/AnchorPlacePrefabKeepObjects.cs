using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorPlacePrefabKeepObjects : AnchorPlacePrefab
{
    [Header("Save active assets on hide")]
    [SerializeField] string tagAssets;
    [SerializeField] int saveCount;
    List<GameObject> objectsToKeep = new List<GameObject>();

    public override void ShowButton()
    {
#if UNITY_EDITOR
        //anchoredAsset = Instantiate(anchorPrefab, new Vector3(0, 0, 4), Quaternion.identity);
#endif

        buttonIsActive = true;

        if (anchorGeo && anchoredAsset == null)
        {
            if (objectsToKeep.Count > 0)
            {
                foreach (var asset in objectsToKeep)
                {
                    Destroy(asset.gameObject);
                }
            }
            objectsToKeep.Clear();

            anchoredAsset = Instantiate(anchorPrefab, anchorGeo.transform);
            anchoredAsset.transform.position = anchorGeo.transform.position;
        }

        if (anchorGeo == null && anchoredAsset == null)
        {
            UiManager.instance.locationServiceMessage.text = "Wait for location!";
        }

        if (locationServiceFailure && anchoredAsset == null)
        {
            UiManager.instance.ShowMessage("tap to place experience!");

            //Exchange update function to coroutine
            StartCoroutine(PlaceWithPlaneTracking());
        }
        else if(locationServiceFailure)
        {
            anchoredAsset.SetActive(true);
        }
    }

    public override void HideButton()
    {
#if UNITY_EDITOR
        Destroy(anchoredAsset);
#endif

        buttonIsActive = false;

        if (anchoredAsset == null) return;

        if (!locationServiceFailure)
        {
            KeepSomeObjects();
            Destroy(anchoredAsset);
        }
        else if (locationServiceFailure)
        {
            anchoredAsset.SetActive(false);
        }
    }

    void KeepSomeObjects()
    {
        objectsToKeep.AddRange(GameObject.FindGameObjectsWithTag(this.tagAssets));

        if (objectsToKeep.Count == 0) return;

        List<int> indexes = new List<int>();
        for (int i = 0; i < saveCount; i++)
        {
            indexes.Add(Random.Range(0, objectsToKeep.Count - 1));
        }

        foreach (int index in indexes)
        {
            objectsToKeep[index].transform.parent = null;
        }
    }
}
