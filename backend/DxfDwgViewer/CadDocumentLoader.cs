using System;
using System.Collections.Generic;
using System.IO;
using ACadSharp;
using ACadSharp.Entities;
using ACadSharp.IO;

namespace DxfDwgViewer
{
    public class CadDocumentLoader
    {
        public CadDocument LoadDocument(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("File name cannot be null or empty", nameof(fileName));

            if (!File.Exists(fileName))
                throw new FileNotFoundException("File not found", fileName);

            Console.WriteLine($"Opening file: {fileName}");

            CadDocument document;
            
            // Load CAD document
            if (fileName.ToLower().EndsWith(".dxf"))
            {
                document = DxfReader.Read(fileName);
            }
            else if (fileName.ToLower().EndsWith(".dwg"))
            {
                document = DwgReader.Read(fileName);
            }
            else
            {
                throw new NotSupportedException("Only DXF or DWG files are supported");
            }

            Console.WriteLine($"Loaded document with {document.Entities.Count()} entities");
            return document;
        }

        public List<Entity> ExtractEntities(CadDocument document)
        {
            var entities = new List<Entity>();
            
            // Extract entities
            foreach (var entity in document.Entities)
            {
                entities.Add(entity);
            }
            
            Console.WriteLine($"Added {entities.Count} entities to list");
            return entities;
        }
    }
}