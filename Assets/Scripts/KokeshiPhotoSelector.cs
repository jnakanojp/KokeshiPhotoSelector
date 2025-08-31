using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class KokeshiPhotoSelector : MonoBehaviour
{
    // Root folder path containing nested subfolders with images.
    // This can be changed from the Inspector.
    public string rootPath = "/Users/Shared/KokeshiImages";

    private readonly List<string> directories = new List<string>();
    private int currentDirIndex = 0;

    private readonly List<string> images = new List<string>();
    private int currentImageIndex = 0;

    private Texture2D currentTexture;
    private string currentImagePath = string.Empty;

    void Start()
    {
        Camera.main.backgroundColor = Color.black;

        if (Directory.Exists(rootPath))
        {
            foreach (var dir in Directory.GetDirectories(rootPath))
            {
                foreach (var sub in Directory.GetDirectories(dir))
                {
                    directories.Add(sub);
                }
            }
        }

        if (directories.Count > 0)
        {
            LoadDirectory(currentDirIndex);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentDirIndex = (currentDirIndex - 1 + directories.Count) % directories.Count;
            LoadDirectory(currentDirIndex);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentDirIndex = (currentDirIndex + 1) % directories.Count;
            LoadDirectory(currentDirIndex);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentImageIndex = (currentImageIndex - 1 + images.Count) % images.Count;
            LoadImage(currentImageIndex);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentImageIndex = (currentImageIndex + 1) % images.Count;
            LoadImage(currentImageIndex);
        }
    }

    private void LoadDirectory(int index)
    {
        images.Clear();
        currentImageIndex = 0;
        if (directories.Count == 0) return;

        string dir = directories[index];
        foreach (var f in Directory.GetFiles(dir))
        {
            string ext = Path.GetExtension(f).ToLowerInvariant();
            if (ext == ".png" || ext == ".jpg" || ext == ".jpeg")
            {
                images.Add(f);
            }
        }

        if (images.Count > 0)
        {
            LoadImage(currentImageIndex);
        }
        else
        {
            currentTexture = null;
            currentImagePath = string.Empty;
        }
    }

    private void LoadImage(int index)
    {
        if (images.Count == 0) return;

        currentImagePath = images[index];
        byte[] data = File.ReadAllBytes(currentImagePath);
        currentTexture = new Texture2D(2, 2);
        currentTexture.LoadImage(data);
    }

    void OnGUI()
    {
        if (!string.IsNullOrEmpty(currentImagePath))
        {
            GUI.Label(new Rect(10, 10, Screen.width - 20, 20), currentImagePath);
            if (currentTexture != null)
            {
                float imgAspect = (float)currentTexture.width / currentTexture.height;
                float screenAspect = (float)Screen.width / Screen.height;
                Rect rect;
                if (imgAspect > screenAspect)
                {
                    float height = Screen.width / imgAspect;
                    rect = new Rect(0, (Screen.height - height) / 2, Screen.width, height);
                }
                else
                {
                    float width = Screen.height * imgAspect;
                    rect = new Rect((Screen.width - width) / 2, 0, width, Screen.height);
                }
                GUI.DrawTexture(rect, currentTexture, ScaleMode.ScaleToFit);
            }
        }
    }
}

