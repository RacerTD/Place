using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public partial class PlaceTextureProcessor : MonoBehaviour
{
    [SerializeField] private Texture2D _texture;
    [SerializeField] private Material _material;
    // [SerializeField] private MeshRenderer _cubeRenderer;

    [Header("Configuration")]
    [SerializeField] private bool _do2017 = true;
    [SerializeField] private PlaceMode _placeMode;
    [SerializeField] private GameObject _tilePrefab;
    [SerializeField] private int _scale;

    [Header("Mode Initialize")]

    [Header("Mode ConstantRate")]
    [Tooltip("The rate in pixels per second to show the place")]
    [SerializeField] private float _rate = 1000f;

    [Header("Normal")]
    [SerializeField] private float _speedModifier = 1f;
    [SerializeField] private double _passedTime = 0f;

    [Header("Mode BeginDelete")]
    private Coordinate[] _deleteArray;

    [Header("Delete")]
    [Tooltip("The rate in pixels per second to delete the place")]
    [SerializeField] private float _deletionRate = 1000f;

    [Header("2017")]
    [SerializeField] private Color[] _colors2017;
    [Header("2022")]
    [SerializeField] private Color[] _colors2022;

    [Header("Runtime - Do not change")]
    [SerializeField] private PlaceMode _currentPlaceMode = PlaceMode.Initialize;
    private List<PlaceDataset> _currentCoordinates = new List<PlaceDataset>();
    [SerializeField] private int _placedPixels;
    [SerializeField] private float _progress;
    [SerializeField] private const int TOTALPIXELS2017 = 15560330;
    private int _currentFileIndex;
    private int _currentIndex;
    private int _currentSubIndex;
    private List<GameObject> _tiles = new List<GameObject>();

    void Start()
    {

    }

    void Update()
    {
        switch (_currentPlaceMode)
        {
            case PlaceMode.Initialize:
                SetupTiles();

                _currentSubIndex = 0;
                _currentIndex = 0;
                _currentFileIndex = 0;
                _placedPixels = 0;

                _texture = new Texture2D(1024, 1024);
                _texture.filterMode = FilterMode.Point;

                // Coloring the texture white
                for (int i = 0; i < 1024; i++)
                {
                    for (int j = 0; j < 1024; j++)
                    {
                        _texture.SetPixel(i, j, _colors2017[0]);
                        // _texture.SetPixel(i, j, ((i + j) % 2 == 1) ? _colors2017[0] : _colors2017[3]);
                    }
                }

                _texture.Apply();

                _material.mainTexture = _texture;
                // _cubeRenderer.material.mainTexture = _texture;

                LoadNextDataset();
                _currentPlaceMode = _placeMode;
                break;

            case PlaceMode.ConstantRate:

                for (int i = 0; i < _rate * Time.deltaTime; i++)
                {
                    _currentSubIndex++;

                    if (_currentSubIndex >= _currentCoordinates[_currentIndex].ChangeList.Count)
                    {
                        _currentSubIndex = 0;
                        _currentIndex++;
                    }

                    if (_currentIndex >= _currentCoordinates.Count)
                    {
                        if (!LoadNextDataset())
                        {
                            _currentPlaceMode = PlaceMode.Idle;
                        }
                    }

                    // Debug.Log($"Index: {_currentIndex}, _currentCoordinates.Count: {_currentCoordinates.Count}, _currentFileIndex: {_currentFileIndex}");
                    UpdatePixel(_currentCoordinates[_currentIndex].ChangeList[_currentSubIndex]);
                }
                _texture.Apply();

                break;

            case PlaceMode.Normal:

                _passedTime += (double)(Time.deltaTime * _speedModifier);
                long nextTimeStamp = _currentCoordinates[_currentIndex].Ticks;

                // Do pixels here
                if (_passedTime >= nextTimeStamp)
                {
                    foreach (Coordinate coordinate in _currentCoordinates[_currentIndex].ChangeList)
                    {
                        UpdatePixel(coordinate);
                    }
                    _texture.Apply();

                    _currentIndex++;
                    if (_currentIndex >= _currentCoordinates.Count)
                    {
                        if (!LoadNextDataset())
                        {
                            _currentPlaceMode = PlaceMode.Idle;
                        }
                    }
                }

                break;

            case PlaceMode.BeginDelete:
                break;

            case PlaceMode.Delete:
                break;

            case PlaceMode.Idle:
                break;
        }
    }

    /// <summary>
    /// Sets up the tiles that display the place
    /// This automatically scales everything to the correct size for any screen, so that no pixel ever gets cut off or is blurry
    /// </summary>
    private void SetupTiles()
    {
        // Deleting existing tiles
        foreach (GameObject gm in _tiles)
        {
            Destroy(gm);
        }
        _tiles.Clear();

        // Dimensions of one tile is 10x10
        // Scaling this gameObject
        float scale = 1000f / (float)Screen.currentResolution.height * (float)_scale;
        transform.localScale = new Vector3(scale, scale, scale);

        // Place new tiles
        // Lol this was generated by chatGPT
        int xTiles = 4;
        int yTiles = 3;
        float offsetX = (xTiles - 1) * 5;
        float offsetY = (yTiles - 1) * 5;
        for (int y = 0; y < yTiles; y++)
        {
            for (int x = 0; x < xTiles; x++)
            {
                _tiles.Add(Instantiate(_tilePrefab, new Vector3(0, 0, 0), Quaternion.Euler(270, 0, 0), this.transform));
                _tiles[_tiles.Count - 1].transform.localPosition = new Vector3(-offsetX + x * 10, offsetY - y * 10, 0);
            }
        }
    }

    /// <summary>
    /// Updates one pixel in the texture
    /// The texture still needs to be applied manually somewhere else in the script
    /// </summary>
    /// <param name="coordinate"></param>
    private void UpdatePixel(Coordinate coordinate)
    {
        _texture.SetPixel(coordinate.X, coordinate.Y, _do2017 ? _colors2017[coordinate.Color] : _colors2022[coordinate.Color]);
        _placedPixels++;
        _progress = ((float)_placedPixels / (float)TOTALPIXELS2017) * 100f;
    }

    /// <summary>
    /// Loads the next dataset
    /// </summary>
    /// <returns></returns>
    private bool LoadNextDataset()
    {
        _currentCoordinates.Clear();
        _currentFileIndex++;
        _currentIndex = 0;
        _currentSubIndex = 0;

        // Assets/Resources/2017/0.csv

        string filePath = (_do2017 ? "2017/" : "2022/") + _currentFileIndex.ToString();
        TextAsset file = Resources.Load<TextAsset>(filePath);

        if (file == null
            || string.IsNullOrEmpty(file.text))
        {
            // Debug.Log($"Reached end of data, FilePath: {filePath}");
            return false;
        }

        // Debug.Log($"filepath: {filePath}, csvFile == null: {csvFile == null}, csvFile.dataSize: {csvFile.dataSize}");

        string[] data = file.text.Split(new char[] { '\n' });

        for (int i = 0; i < data.Length; i++)
        {
            if (string.IsNullOrEmpty(data[i]))
                continue;

            string[] splitDataSet = data[i].Split(";");
            long timeStamp = long.Parse(splitDataSet[0]);
            List<Coordinate> tempCoordinates = new List<Coordinate>();

            for (int j = 2; j < splitDataSet.Length; j++)
            {
                string[] splitCoordinate = splitDataSet[j].Split(",");
                tempCoordinates.Add(new Coordinate(short.Parse(splitCoordinate[0]), short.Parse(splitCoordinate[1]), byte.Parse(splitCoordinate[2])));
            }

            _currentCoordinates.Add(new PlaceDataset(timeStamp, tempCoordinates));
        }

        return true;
    }
}
