using Akira.Models.Processing;
using System.Collections.Generic;

namespace Akira.Models
{
    public class Denormalizer
    {
        public static List<Mesh> Denormalize(FileFormatWavefront.Model.Scene scene)
        {
            var meshes = new List<Mesh>();
            var vertics = scene.Vertices;
            var normals = scene.Normals;
            var uvs = scene.Uvs;

            List<Processing.Face> facesWithSameIndexCount = new List<Processing.Face>();
            int currentIndexCount = -1;

            // Проходимся по каждой группе и денормализируем её
            foreach (var group in scene.Groups)
            {
                // Проходимся по каждому фейсу
                for (int i = 0; i < group.Faces.Count; i++)
                {
                    var face = group.Faces[i];

                    // Если это первый фейс, то устанавливаем текущий счетчик индексов
                    if (currentIndexCount == -1)
                    {
                        currentIndexCount = face.Indices.Count;
                    }
                    else if (currentIndexCount == face.Indices.Count)
                    {
                        //facesWithSameIndexCount.Add(face);
                    }

                    // Если это новый отсчет индекса или конец, то завершаем сетку
                    if (currentIndexCount != face.Indices.Count || i == group.Faces.Count - 1)
                    {
                        //var indices = facesWithSameIndexCount.SelectMany(f => f.Indices).ToList();

                        // Создаем вершины сетки
                        //var vs = indices.Select(ind => vertics[ind.vertex]).Select(v => new vec3(v.x, v.y, v.z)).ToArray();

                        //var ns = indices.Any(ind => ind.normal.HasValue == false) ? null
                        //            : indices.Select(ind => normals[ind.normal.Value])
                        //            .Select(v => new vec3(v.x, v.y, v.z)).ToArray();

                        //var ts = indices.Any(ind => ind.normal.HasValue == false) ? null
                        //            : indices.Select(ind => uvs[ind.uv.Value])
                        //            .Select(v => new vec2(v.u, v.v)).ToArray();

                        meshes.Add(new Mesh
                        {
                            //vertices = vs,
                            //normals = ns,
                            //uvs = ts,
                            //material = facesWithSameIndexCount.First().Material,
                            indicesPerFace = currentIndexCount
                        });

                        //facesWithSameIndexCount = new List<Face>();
                        //facesWithSameIndexCount.Add(face);
                        currentIndexCount = face.Indices.Count;
                    }
                }
            }

            return meshes;
        }
    }
}
