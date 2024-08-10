using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MergeMeshes : MonoBehaviour
{
    public enum DisbleType
    {
        Full,
        ComponentOnly,
        None
    }
    public bool useColider = false;
    public bool includeInactive = false;
    public DisbleType disble;
    public void mergeMeshes()
    {
        Vector3 curentWorldPosition = transform.position;
        transform.position = Vector3.zero;

        MeshFilter curentFilter = GetComponent<MeshFilter>();
        MeshRenderer curentRendrer = GetComponent<MeshRenderer>();
        MeshCollider curentColider = GetComponent<MeshCollider>();
        if (curentFilter == null)
            curentFilter = gameObject.AddComponent<MeshFilter>();
        if (curentRendrer == null)
            curentRendrer = gameObject.AddComponent<MeshRenderer>();
        if (curentColider == null && useColider)
            curentColider = gameObject.AddComponent<MeshCollider>();


        MeshFilter[] filters = GetComponentsInChildren<MeshFilter>(includeInactive);
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>(includeInactive);

        Dictionary<Material, List<CombineInstance>> submeshes = new Dictionary<Material, List<CombineInstance>>(); 
        for(int i=0;i<filters.Length;i++)
        {
            if (filters[i].gameObject == gameObject || !renderers[i].enabled)
                continue;
            Material[] materials = renderers[i].sharedMaterials;
            for(int k = 0; k < materials.Length; k++)
            {
                if (!submeshes.ContainsKey(materials[k]))
                {
                    submeshes.Add(materials[k], new List<CombineInstance>());
                }

                CombineInstance ci = new CombineInstance();
                ci.mesh = filters[i].sharedMesh;
                ci.subMeshIndex = k;
                ci.transform = filters[i].transform.localToWorldMatrix;
                submeshes[materials[k]].Add(ci);
            }
        }

        List<CombineInstance> newSubmeshes = new List<CombineInstance>();
        List<Material> materials_finale = new List<Material>();
        foreach(Material m in submeshes.Keys)
        {
            List<CombineInstance> l_ci = submeshes[m];
            Mesh submesh = new Mesh();
            submesh.CombineMeshes(l_ci.ToArray(), true);

            CombineInstance ci = new CombineInstance();
            ci.mesh = submesh;
            ci.subMeshIndex = 0;
            ci.transform = Matrix4x4.identity;
            materials_finale.Add(m);
            newSubmeshes.Add(ci);

            //curentFilter.mesh = submesh;
            //Debug.Log(submesh.triangles.Length);
        }

        Mesh finalMesh = new Mesh();
        finalMesh.CombineMeshes(newSubmeshes.ToArray(), false);
        
        curentFilter.sharedMesh = finalMesh;
        curentRendrer.sharedMaterials = materials_finale.ToArray();
        if(useColider)
            curentColider.sharedMesh = finalMesh;
        
        for(int i=0;i<renderers.Length;i++)
        {
            if(renderers[i].gameObject != gameObject)
            {
                if (disble == DisbleType.Full)
                    renderers[i].gameObject.SetActive(false);
                else if(disble == DisbleType.ComponentOnly)
                    renderers[i].enabled = false;
            }
        }

        transform.position = curentWorldPosition;
    }

    public void enableAll()
    {
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>(true);
        for(int i = 0; i < renderers.Length; i++)
        {
            renderers[i].enabled = true;
            renderers[i].gameObject.SetActive(true);
        }
    }
}
