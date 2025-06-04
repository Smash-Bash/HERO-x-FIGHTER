using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public MultiplayerManager multiplayer;
    public Camera camera;
    public float size = 5;
    public float cameraDistance = -10;
    public Vector2 cameraShake;
    public Vector3 spawnPoint;
    public float spawnSize;

    // Start is called before the first frame update
    void Start()
    {
        spawnPoint = transform.position;
        spawnSize = camera.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate()
    {
        cameraShake = Vector2.Lerp(cameraShake, Vector2.zero, Time.deltaTime * 15);
        if (Time.timeScale > 0)
        {
            CameraUpdate();
        }
    }

    public void CameraUpdate()
    {
        CinematicCamera[] cinematicCameras = FindObjectsOfType<CinematicCamera>();



        Vector3 position = Vector3.zero;
        float newSize = size;
        if (multiplayer.endOfRound)
        {
            newSize = 2.5f;
        }
        float smallestSize = Mathf.Min(multiplayer.blastZone.x, multiplayer.blastZone.y);
        if (multiplayer.endOfRound)
        {
            smallestSize = 2.5f;
        }
        foreach (PlayerScript player in multiplayer.players)
        {
            if (!player.unconscious)
            {
                position += player.transform.position;
                foreach (PlayerScript otherPlayer in multiplayer.players)
                {
                    newSize = Mathf.Max(newSize, (Vector3.Distance(player.transform.position, otherPlayer.transform.position) / 2) + 2.5f - size);
                    newSize = Mathf.Min(newSize, smallestSize / 2.25f);
                }
            }
        }

        if (multiplayer.alivePlayers > 0)
        {
            position /= multiplayer.alivePlayers;
        }

        position = new Vector3(position.x, position.y, cameraDistance - ((camera.orthographicSize - size) * 2));

        float sizeX = (camera.orthographicSize * 2) * (camera.aspect);
        float sizeY = camera.orthographicSize * 2;

        if (camera.orthographic)
        {
            position = new Vector3(Mathf.Clamp(position.x, (multiplayer.blastZone.x * -0.5f) + (sizeX / 2), (multiplayer.blastZone.x * 0.5f) - (sizeX / 2)), Mathf.Clamp(position.y, (multiplayer.blastZone.y * -0.5f) + (sizeY / 2), (multiplayer.blastZone.y * 0.5f) - (sizeY / 2)), cameraDistance - ((camera.orthographicSize - size) * 2));
        }

        transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * 5);
        if (camera.orthographic)
        {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, (multiplayer.blastZone.x * -0.5f) + (sizeX / 2), (multiplayer.blastZone.x * 0.5f) - (sizeX / 2)), Mathf.Clamp(transform.position.y, (multiplayer.blastZone.y * -0.5f) + (sizeY / 2), (multiplayer.blastZone.y * 0.5f) - (sizeY / 2)), cameraDistance - ((camera.orthographicSize - size) * 2));

            camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, Mathf.Clamp(newSize, size, smallestSize), Time.deltaTime * 5);
        }

        if (multiplayer.endOfRound)
        {
            camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, 2.5f, Time.deltaTime * 5);
        }
        else
        {
            camera.orthographicSize = Mathf.Clamp(newSize, size, smallestSize);
            camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, 0, multiplayer.blastZone.x / 2);
        }

        transform.position += new Vector3(Random.Range(cameraShake.x * -1f, cameraShake.x * 1f), Random.Range(cameraShake.y * -1f, cameraShake.y * 1f), 0);

        foreach (CinematicCamera currentCamera in cinematicCameras)
        {
            if (currentCamera.isActiveAndEnabled)
            {
                transform.position = currentCamera.transform.position;
                transform.rotation = currentCamera.transform.rotation;
                camera.orthographic = currentCamera.camera.orthographic;
                camera.orthographicSize = currentCamera.camera.orthographicSize;
                camera.fieldOfView = currentCamera.camera.fieldOfView;
            }
        }
    }

    public void ShakeCamera(Vector2 shakeAmount)
    {
        if (shakeAmount.magnitude > cameraShake.magnitude)
        {
            cameraShake = shakeAmount;
        }
    }
}
