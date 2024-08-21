﻿using UnityEngine;

public class RainsplashBox : MonoBehaviour
{
    private MeshFilter mf;
    private Bounds bounds;

    private RainsplashManager manager;

    private void Start()
    {
        transform.localRotation = Quaternion.identity;

        manager = transform.parent.GetComponent<RainsplashManager>();
        bounds = new Bounds(new Vector3(transform.position.x, 0.0f, transform.position.z),
                            new Vector3(manager.areaSize, Mathf.Max(manager.areaSize, manager.areaHeight), manager.areaSize));

        mf = GetComponent<MeshFilter>();
        mf.sharedMesh = manager.GetPreGennedMesh();

        enabled = false;
    }

    private void OnBecameVisible()
    {
        enabled = true;
    }

    private void OnBecameInvisible()
    {
        enabled = false;
    }

    private void OnDrawGizmos()
    {
        if (transform.parent)
        {
            manager = transform.parent.GetComponent<RainsplashManager>();
            Gizmos.color = new Color(0.5f, 0.5f, 0.65f, 0.5f);
            if (manager)
                Gizmos.DrawWireCube(transform.position + transform.up * manager.areaHeight * 0.5f,
                                    new Vector3(manager.areaSize, manager.areaHeight, manager.areaSize));
        }
    }
}
