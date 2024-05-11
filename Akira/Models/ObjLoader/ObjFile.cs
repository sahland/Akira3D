using Akira.Models.Processing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Akira.Models.ObjLoader
{
    public class ObjFile
    {
        public List<Vertex> Vertices { get; private set; } = new List<Vertex>();
        public List<TextureVertex> TextureCoordinates { get; private set; } = new List<TextureVertex>();
        public List<Normal> Normals { get; private set; } = new List<Normal>();
        public List<Group> Groups { get; private set; } = new List<Group>();

        public string MaterialLibrary { get; private set; }

        public void Load(string filename)
        {
            try
            {
                Vertices.Clear();
                TextureCoordinates.Clear();
                Normals.Clear();
                Groups.Clear();

                Group currentGroup = null;
                string currentMaterialLibrary = null;
                if (filename != "")
                {
                    Console.WriteLine(filename);
                    using (FileStream fs = new FileStream(filename, FileMode.Open))
                    {
                        using (StreamReader sr = new StreamReader(fs))
                        {
                            string line;
                            while ((line = sr.ReadLine()) != null)
                            {

                                //    foreach (var line in File.ReadLines(filename))

                                if (string.IsNullOrWhiteSpace(line))
                                {
                                    continue;
                                }

                                var parts = line.Trim().Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                                if (parts.Length == 0)
                                {
                                    continue;
                                }

                                switch (parts[0])
                                {
                                    case "#":
                                        // Комментарий
                                        break;
                                    case "v":
                                        // Вершина
                                        var x = float.Parse(parts[1], CultureInfo.InvariantCulture);
                                        var y = float.Parse(parts[2], CultureInfo.InvariantCulture);
                                        var z = float.Parse(parts[3], CultureInfo.InvariantCulture);
                                        Vertices.Add(new Vertex(x, y, z));
                                        break;
                                    case "vt":
                                        // Текстурная координата
                                        var u = float.Parse(parts[1], CultureInfo.InvariantCulture);
                                        var v = float.Parse(parts[2], CultureInfo.InvariantCulture);
                                        TextureCoordinates.Add(new TextureVertex(u, v));
                                        break;
                                    case "vn":
                                        // Нормаль
                                        var nx = float.Parse(parts[1], CultureInfo.InvariantCulture);
                                        var ny = float.Parse(parts[2], CultureInfo.InvariantCulture);
                                        var nz = float.Parse(parts[3], CultureInfo.InvariantCulture);
                                        Normals.Add(new Normal(nx, ny, nz));
                                        break;
                                    case "f":
                                        // Грань
                                        var face = new Face();
                                        for (int i = 1; i < parts.Length; i++)
                                        {
                                            var vertexParts = parts[i].Split('/');
                                            var vertexIndex = int.Parse(vertexParts[0]);
                                            var textureCoordinateIndex = vertexParts.Length > 1 && !string.IsNullOrWhiteSpace(vertexParts[1]) ? int.Parse(vertexParts[1]) : 0;
                                            var normalIndex = vertexParts.Length > 2 && !string.IsNullOrWhiteSpace(vertexParts[2]) ? int.Parse(vertexParts[2]) : 0;

                                            face.VertexIndices.Add(new VertexIndex(vertexIndex, textureCoordinateIndex, normalIndex));
                                        }

                                        if (currentGroup == null)
                                        {
                                            currentGroup = new Group();
                                            Groups.Add(currentGroup);
                                        }

                                        face.MaterialLibrary = currentMaterialLibrary;
                                        face.GroupName = currentGroup.Name;
                                        currentGroup.Faces.Add(face);
                                        break;
                                    case "g":
                                        Groups.Add(currentGroup);
                                        break;
                                    case "usemtl":
                                        currentMaterialLibrary = parts[1];
                                        break;
                                    case "mtllib":
                                        // Загрузка материалов
                                        MaterialLibrary = parts[1];
                                        // Для загрузки материалов используется отдельный класс и не рассматривается в данном коде.
                                        break;
                                    default:
                                        // Неизвестный тип данных
                                        break;
                                }
                            }
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }
    }
}

