using Google.XR.ARCoreExtensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

public class StreetscapeManager : MonoBehaviour
{
    public ARStreetscapeGeometryManager StreetscapeGeometryManager;
    public List<Material> StreetscapeGeometryMaterialBuilding;
    public Material StreetscapeGeometryMaterialTerrain;

    private bool _streetscapeGeometryVisibility = false;
    private int _buildingMatIndex = 0;
    private Dictionary<TrackableId, GameObject> _streetscapegeometryGOs = new Dictionary<TrackableId, GameObject>();
    List<ARStreetscapeGeometry> _addedStreetscapeGeometries = new List<ARStreetscapeGeometry>();
    List<ARStreetscapeGeometry> _updatedStreetscapeGeometries = new List<ARStreetscapeGeometry>();
    List<ARStreetscapeGeometry> _removedStreetscapeGeometries = new List<ARStreetscapeGeometry>();
    private bool _clearStreetscapeGeometryRenderObjects = false;


    public void OnGeometryToggled(bool enabled)
    {
        _streetscapeGeometryVisibility = enabled;
        if (!_streetscapeGeometryVisibility)
        {
            _clearStreetscapeGeometryRenderObjects = true;
        }
    }


    public void OnEnable()
    {
        if (StreetscapeGeometryManager == null)
        {
            Debug.LogWarning("StreetscapeGeometryManager must be set in the " +
                "GeospatialController Inspector to render StreetscapeGeometry.");
        }

        if (StreetscapeGeometryMaterialBuilding.Count == 0)
        {
            Debug.LogWarning("StreetscapeGeometryMaterialBuilding in the " +
                "GeospatialController Inspector must contain at least one material " +
                "to render StreetscapeGeometry.");
            return;
        }

        if (StreetscapeGeometryMaterialTerrain == null)
        {
            Debug.LogWarning("StreetscapeGeometryMaterialTerrain must be set in the " +
                "GeospatialController Inspector to render StreetscapeGeometry.");
            return;
        }

        // get access to ARstreetscapeGeometries in ARStreetscapeGeometryManager
        if (StreetscapeGeometryManager)
        {
            StreetscapeGeometryManager.StreetscapeGeometriesChanged += GetStreetscapeGeometry;
        }
    }

    private void Start()
    {
        CheckLocationService.Instance.onLocationServiceSuccess.AddListener(OnLocationServiceSuccess);
    }


    private void OnLocationServiceSuccess()
    {
        if (_streetscapeGeometryVisibility)
        {
            foreach (
                ARStreetscapeGeometry streetscapegeometry in _addedStreetscapeGeometries)
            {
                InstantiateRenderObject(streetscapegeometry);
            }

            foreach (
                ARStreetscapeGeometry streetscapegeometry in _updatedStreetscapeGeometries)
            {
                // This second call to instantiate is required if geometry is toggled on
                // or off after the app has started.
                InstantiateRenderObject(streetscapegeometry);
                UpdateRenderObject(streetscapegeometry);
            }

            foreach (
                ARStreetscapeGeometry streetscapegeometry in _removedStreetscapeGeometries)
            {
                DestroyRenderObject(streetscapegeometry);
            }
        }
        else if (_clearStreetscapeGeometryRenderObjects)
        {
            DestroyAllRenderObjects();
            _clearStreetscapeGeometryRenderObjects = false;
        }
    }


    private void InstantiateRenderObject(ARStreetscapeGeometry streetscapegeometry)
    {
        if (streetscapegeometry.mesh == null)
        {
            return;
        }

        // Check if a render object already exists for this streetscapegeometry and
        // create one if not.
        if (_streetscapegeometryGOs.ContainsKey(streetscapegeometry.trackableId))
        {
            return;
        }

        GameObject renderObject = new GameObject(
            "StreetscapeGeometryMesh", typeof(MeshFilter), typeof(MeshRenderer));

        if (renderObject)
        {
            renderObject.transform.position = new Vector3(0, 0.5f, 0);
            renderObject.GetComponent<MeshFilter>().mesh = streetscapegeometry.mesh;

            // Add a material with transparent diffuse shader.
            if (streetscapegeometry.streetscapeGeometryType ==
                StreetscapeGeometryType.Building)
            {
                renderObject.GetComponent<MeshRenderer>().material =
                    StreetscapeGeometryMaterialBuilding[_buildingMatIndex];
                _buildingMatIndex =
                    (_buildingMatIndex + 1) % StreetscapeGeometryMaterialBuilding.Count;
            }
            else
            {
                renderObject.GetComponent<MeshRenderer>().material =
                    StreetscapeGeometryMaterialTerrain;
            }

            renderObject.transform.position = streetscapegeometry.pose.position;
            renderObject.transform.rotation = streetscapegeometry.pose.rotation;

            _streetscapegeometryGOs.Add(streetscapegeometry.trackableId, renderObject);
        }
    }


    private void UpdateRenderObject(ARStreetscapeGeometry streetscapegeometry)
    {
        if (_streetscapegeometryGOs.ContainsKey(streetscapegeometry.trackableId))
        {
            GameObject renderObject = _streetscapegeometryGOs[streetscapegeometry.trackableId];
            renderObject.transform.position = streetscapegeometry.pose.position;
            renderObject.transform.rotation = streetscapegeometry.pose.rotation;
        }
    }


    private void DestroyRenderObject(ARStreetscapeGeometry streetscapegeometry)
    {
        if (_streetscapegeometryGOs.ContainsKey(streetscapegeometry.trackableId))
        {
            var geometry = _streetscapegeometryGOs[streetscapegeometry.trackableId];
            _streetscapegeometryGOs.Remove(streetscapegeometry.trackableId);
            Destroy(geometry);
        }
    }


    private void DestroyAllRenderObjects()
    {
        var keys = _streetscapegeometryGOs.Keys;
        foreach (var key in keys)
        {
            var renderObject = _streetscapegeometryGOs[key];
            Destroy(renderObject);
        }

        _streetscapegeometryGOs.Clear();
    }


    private void GetStreetscapeGeometry(ARStreetscapeGeometriesChangedEventArgs eventArgs)
    {
        _addedStreetscapeGeometries = eventArgs.Added;
        _updatedStreetscapeGeometries = eventArgs.Updated;
        _removedStreetscapeGeometries = eventArgs.Removed;
    }
}
