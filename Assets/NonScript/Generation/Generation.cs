using UnityEngine;

public struct GenerationProperties
{
    public int pointsLenghth;
    public Vector3Int[] points;
    public int[,,] points_distance;

    //public void CreateDirections()
    //{
    //    for (int i = 0; i < points.Length; i++)
    //    {
    //        points_distance[points[i].x, points[i].y, points[i].z] = 0;
    //    }
    //}
    //public void AddDirection(int x, int y, int z)
    //{
    //    UpdateDirections(0, x, y, z);
    //}
    //public void RemoveDirection(int x, int y, int z)
    //{
    //    if (CheckDirection(x, y, z) || points_distance[x,y,z] == int.MaxValue)
    //        return;
    //    if (IsDirectionValid(x, y, z))
    //    {
    //        UpdateDirections(points_distance[x, y, z] - (int)GenerationProp.PointOIDirectionMultiplayer.x, x + 1, y, z);
    //        UpdateDirections(points_distance[x, y, z] - (int)GenerationProp.PointOIDirectionMultiplayer.x, x - 1, y, z);
    //        UpdateDirections(points_distance[x, y, z] - (int)GenerationProp.PointOIDirectionMultiplayer.y, x, y + 1, z);
    //        UpdateDirections(points_distance[x, y, z] - (int)GenerationProp.PointOIDirectionMultiplayer.y, x, y - 1, z);
    //        UpdateDirections(points_distance[x, y, z] - (int)GenerationProp.PointOIDirectionMultiplayer.z, x, y, z + 1);
    //        UpdateDirections(points_distance[x, y, z] - (int)GenerationProp.PointOIDirectionMultiplayer.z, x, y, z - 1);
    //        return;
    //    }
    //    points_distance[x, y, z] = int.MaxValue;
    //    RemoveDirection(x + 1, y, z);
    //    RemoveDirection(x - 1, y, z);
    //    RemoveDirection(x, y + 1, z);
    //    RemoveDirection(x, y - 1, z);
    //    RemoveDirection(x, y, z + 1);
    //    RemoveDirection(x, y, z - 1);
    //}
    //void UpdateDirections(int value, int x, int y, int z)
    //{
    //    if (CheckDirection(x, y, z) || points_distance[x, y, z] <= value)
    //        return;
    //    points_distance[x, y, z] = value;

    //    UpdateDirections(value - (int)GenerationProp.PointOIDirectionMultiplayer.x, x + 1, y, z);
    //    UpdateDirections(value - (int)GenerationProp.PointOIDirectionMultiplayer.x, x - 1, y, z);
    //    UpdateDirections(value - (int)GenerationProp.PointOIDirectionMultiplayer.y, x, y + 1, z);
    //    UpdateDirections(value - (int)GenerationProp.PointOIDirectionMultiplayer.y, x, y - 1, z);
    //    UpdateDirections(value - (int)GenerationProp.PointOIDirectionMultiplayer.z, x, y, z + 1);
    //    UpdateDirections(value - (int)GenerationProp.PointOIDirectionMultiplayer.z, x, y, z - 1);
    //}
    //bool IsDirectionValid(int x, int y, int z)
    //{
    //    if (
    //        points_distance[x + 1, y, z] == points_distance[x, y, z] - GenerationProp.PointOIDirectionMultiplayer.x ||
    //        points_distance[x - 1, y, z] == points_distance[x, y, z] - GenerationProp.PointOIDirectionMultiplayer.x ||
    //        points_distance[x, y + 1, z] == points_distance[x, y, z] - GenerationProp.PointOIDirectionMultiplayer.y ||
    //        points_distance[x, y - 1, z] == points_distance[x, y, z] - GenerationProp.PointOIDirectionMultiplayer.y ||
    //        points_distance[x, y, z + 1] == points_distance[x, y, z] - GenerationProp.PointOIDirectionMultiplayer.z ||
    //        points_distance[x, y, z - 1] == points_distance[x, y, z] - GenerationProp.PointOIDirectionMultiplayer.z
    //       )
    //        return true;
    //    return false;
    //}
    //public bool CheckDirection(int x, int y, int z)
    //{
    //    if (x < 0 || y < 0 || z < 0 || x >= points_distance.GetLength(0) || y >= points_distance.GetLength(1) || z >= points_distance.GetLength(2))
    //        return false;
    //    return true;
    //}
}
