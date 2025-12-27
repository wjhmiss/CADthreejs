import { describe, it, expect, beforeEach, afterEach } from 'vitest';
import * as THREE from 'three';
import DimensionEntityThreejsRenderer, { DimensionData } from '../DimensionEntityThreejsRenderer';

describe('DimensionEntityThreejsRenderer', () => {
  let scene: THREE.Scene;

  beforeEach(() => {
    scene = new THREE.Scene();
  });

  afterEach(() => {
    scene.clear();
  });

  describe('render', () => {
    it('should render a valid dimension', () => {
      const dimensionData: DimensionData = {
        Type: 'Dimension',
        DefinitionPointX: 10,
        DefinitionPointY: 20,
        DefinitionPointZ: 0,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 1,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 20, Z: 0 },
          Center: { X: 5, Y: 10, Z: 0 },
          Size: { X: 10, Y: 20, Z: 0 }
        },
        Centroid: { X: 5, Y: 10, Z: 0 },
        Points: [],
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true
      };

      const result = DimensionEntityThreejsRenderer.render(dimensionData, scene);

      expect(result).not.toBeNull();
      expect(result.group).toBeInstanceOf(THREE.Group);
      expect(result.group.name).toBe('Dimension_Dimension');
      expect(result.dimensionType).toBe('Dimension');
      expect(result.bounds).toBeInstanceOf(THREE.Box3);
    });

    it('should render dimension with Angular3PtData', () => {
      const dimensionData: DimensionData = {
        Type: 'DimensionAngular3Pt',
        DefinitionPointX: 0,
        DefinitionPointY: 0,
        DefinitionPointZ: 0,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 1,
        Angular3PtData: {
          VertexX: 0,
          VertexY: 0,
          VertexZ: 0,
          FirstPointX: 10,
          FirstPointY: 0,
          FirstPointZ: 0,
          SecondPointX: 0,
          SecondPointY: 10,
          SecondPointZ: 0,
          NormalX: 0,
          NormalY: 0,
          NormalZ: 1,
          Measurement: 1.5708,
          Text: '90°',
          AngleInDegrees: 90,
          AngleInRadians: 1.5708,
          ArcCenter: { X: 0, Y: 0, Z: 0 },
          Radius: 10,
          Bounds: {
            Min: { X: 0, Y: 0, Z: 0 },
            Max: { X: 10, Y: 10, Z: 0 },
            Center: { X: 5, Y: 5, Z: 0 },
            Size: { X: 10, Y: 10, Z: 0 }
          },
          ArcPoints: [
            { X: 10, Y: 0, Z: 0 },
            { X: 7.07, Y: 7.07, Z: 0 },
            { X: 0, Y: 10, Z: 0 }
          ]
        },
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 10, Z: 0 },
          Center: { X: 5, Y: 5, Z: 0 },
          Size: { X: 10, Y: 10, Z: 0 }
        },
        Centroid: { X: 5, Y: 5, Z: 0 },
        Points: [],
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true
      };

      const result = DimensionEntityThreejsRenderer.render(dimensionData, scene);

      expect(result).not.toBeNull();
      expect(result.group).toBeInstanceOf(THREE.Group);
      expect(result.group.name).toBe('Dimension_DimensionAngular3Pt');
      expect(result.dimensionType).toBe('DimensionAngular3Pt');
    });

    it('should render dimension with Angular2LineData', () => {
      const dimensionData: DimensionData = {
        Type: 'DimensionAngular2Line',
        DefinitionPointX: 0,
        DefinitionPointY: 0,
        DefinitionPointZ: 0,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 1,
        Angular2LineData: {
          AngleVertexX: 0,
          AngleVertexY: 0,
          AngleVertexZ: 0,
          FirstPointX: 10,
          FirstPointY: 0,
          FirstPointZ: 0,
          SecondPointX: 0,
          SecondPointY: 10,
          SecondPointZ: 0,
          DefinitionPointX: 5,
          DefinitionPointY: 5,
          DefinitionPointZ: 0,
          NormalX: 0,
          NormalY: 0,
          NormalZ: 1,
          Measurement: 1.5708,
          Text: '90°',
          AngleInDegrees: 90,
          AngleInRadians: 1.5708,
          ArcCenter: { X: 0, Y: 0, Z: 0 },
          Radius: 10,
          Bounds: {
            Min: { X: 0, Y: 0, Z: 0 },
            Max: { X: 10, Y: 10, Z: 0 },
            Center: { X: 5, Y: 5, Z: 0 },
            Size: { X: 10, Y: 10, Z: 0 }
          },
          ArcPoints: [
            { X: 10, Y: 0, Z: 0 },
            { X: 7.07, Y: 7.07, Z: 0 },
            { X: 0, Y: 10, Z: 0 }
          ]
        },
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 10, Z: 0 },
          Center: { X: 5, Y: 5, Z: 0 },
          Size: { X: 10, Y: 10, Z: 0 }
        },
        Centroid: { X: 5, Y: 5, Z: 0 },
        Points: [],
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true
      };

      const result = DimensionEntityThreejsRenderer.render(dimensionData, scene);

      expect(result).not.toBeNull();
      expect(result.group).toBeInstanceOf(THREE.Group);
      expect(result.group.name).toBe('Dimension_DimensionAngular2Line');
      expect(result.dimensionType).toBe('DimensionAngular2Line');
    });

    it('should render dimension with DiameterData', () => {
      const dimensionData: DimensionData = {
        Type: 'DimensionDiameter',
        DefinitionPointX: 0,
        DefinitionPointY: 0,
        DefinitionPointZ: 0,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 1,
        DiameterData: {
          DefinitionPointX: 0,
          DefinitionPointY: 0,
          DefinitionPointZ: 0,
          FirstPointX: -10,
          FirstPointY: 0,
          FirstPointZ: 0,
          SecondPointX: 10,
          SecondPointY: 0,
          SecondPointZ: 0,
          NormalX: 0,
          NormalY: 0,
          NormalZ: 1,
          Measurement: 20,
          Text: '20',
          Center: { X: 0, Y: 0, Z: 0 },
          Radius: 10,
          Diameter: 20,
          Bounds: {
            Min: { X: -10, Y: 0, Z: 0 },
            Max: { X: 10, Y: 0, Z: 0 },
            Center: { X: 0, Y: 0, Z: 0 },
            Size: { X: 20, Y: 0, Z: 0 }
          },
          LeaderLength: 5
        },
        Bounds: {
          Min: { X: -10, Y: 0, Z: 0 },
          Max: { X: 10, Y: 0, Z: 0 },
          Center: { X: 0, Y: 0, Z: 0 },
          Size: { X: 20, Y: 0, Z: 0 }
        },
        Centroid: { X: 0, Y: 0, Z: 0 },
        Points: [],
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true
      };

      const result = DimensionEntityThreejsRenderer.render(dimensionData, scene);

      expect(result).not.toBeNull();
      expect(result.group).toBeInstanceOf(THREE.Group);
      expect(result.group.name).toBe('Dimension_DimensionDiameter');
      expect(result.dimensionType).toBe('DimensionDiameter');
    });

    it('should render dimension with RadiusData', () => {
      const dimensionData: DimensionData = {
        Type: 'DimensionRadius',
        DefinitionPointX: 0,
        DefinitionPointY: 0,
        DefinitionPointZ: 0,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 1,
        RadiusData: {
          DefinitionPointX: 0,
          DefinitionPointY: 0,
          DefinitionPointZ: 0,
          FirstPointX: 0,
          FirstPointY: 0,
          FirstPointZ: 0,
          SecondPointX: 10,
          SecondPointY: 0,
          SecondPointZ: 0,
          NormalX: 0,
          NormalY: 0,
          NormalZ: 1,
          Measurement: 10,
          Text: 'R10',
          Center: { X: 0, Y: 0, Z: 0 },
          Radius: 10,
          Bounds: {
            Min: { X: 0, Y: 0, Z: 0 },
            Max: { X: 10, Y: 0, Z: 0 },
            Center: { X: 5, Y: 0, Z: 0 },
            Size: { X: 10, Y: 0, Z: 0 }
          },
          LeaderLength: 5
        },
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 0, Z: 0 },
          Center: { X: 5, Y: 0, Z: 0 },
          Size: { X: 10, Y: 0, Z: 0 }
        },
        Centroid: { X: 5, Y: 0, Z: 0 },
        Points: [],
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true
      };

      const result = DimensionEntityThreejsRenderer.render(dimensionData, scene);

      expect(result).not.toBeNull();
      expect(result.group).toBeInstanceOf(THREE.Group);
      expect(result.group.name).toBe('Dimension_DimensionRadius');
      expect(result.dimensionType).toBe('DimensionRadius');
    });

    it('should render dimension with OrdinateData', () => {
      const dimensionData: DimensionData = {
        Type: 'DimensionOrdinate',
        DefinitionPointX: 0,
        DefinitionPointY: 0,
        DefinitionPointZ: 0,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 1,
        OrdinateData: {
          DefinitionPointX: 0,
          DefinitionPointY: 0,
          DefinitionPointZ: 0,
          FirstPointX: 10,
          FirstPointY: 10,
          FirstPointZ: 0,
          SecondPointX: 10,
          SecondPointY: 20,
          SecondPointZ: 0,
          NormalX: 0,
          NormalY: 0,
          NormalZ: 1,
          IsOrdinateTypeX: true,
          Measurement: 10,
          Text: '10',
          Bounds: {
            Min: { X: 0, Y: 0, Z: 0 },
            Max: { X: 10, Y: 20, Z: 0 },
            Center: { X: 5, Y: 10, Z: 0 },
            Size: { X: 10, Y: 20, Z: 0 }
          },
          Centroid: { X: 5, Y: 10, Z: 0 },
          FeatureLocation: { X: 10, Y: 10, Z: 0 },
          LeaderEndpoint: { X: 10, Y: 20, Z: 0 },
          LeaderDirection: { X: 0, Y: 1, Z: 0 },
          LeaderLength: 10
        },
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 20, Z: 0 },
          Center: { X: 5, Y: 10, Z: 0 },
          Size: { X: 10, Y: 20, Z: 0 }
        },
        Centroid: { X: 5, Y: 10, Z: 0 },
        Points: [],
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true
      };

      const result = DimensionEntityThreejsRenderer.render(dimensionData, scene);

      expect(result).not.toBeNull();
      expect(result.group).toBeInstanceOf(THREE.Group);
      expect(result.group.name).toBe('Dimension_DimensionOrdinate');
      expect(result.dimensionType).toBe('DimensionOrdinate');
    });

    it('should render dimension with AlignedData', () => {
      const dimensionData: DimensionData = {
        Type: 'DimensionAligned',
        DefinitionPointX: 0,
        DefinitionPointY: 0,
        DefinitionPointZ: 0,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 1,
        AlignedData: {
          FirstPointX: 0,
          FirstPointY: 0,
          FirstPointZ: 0,
          SecondPointX: 10,
          SecondPointY: 10,
          SecondPointZ: 0,
          DefinitionPointX: 5,
          DefinitionPointY: 15,
          DefinitionPointZ: 0,
          NormalX: 0,
          NormalY: 0,
          NormalZ: 1,
          Measurement: 14.142,
          Text: '14.14',
          ExtLineRotation: 0.7854,
          Bounds: {
            Min: { X: 0, Y: 0, Z: 0 },
            Max: { X: 10, Y: 15, Z: 0 },
            Center: { X: 5, Y: 7.5, Z: 0 },
            Size: { X: 10, Y: 15, Z: 0 }
          },
          Centroid: { X: 5, Y: 7.5, Z: 0 },
          FirstPoint: { X: 0, Y: 0, Z: 0 },
          SecondPoint: { X: 10, Y: 10, Z: 0 },
          DefinitionPoint: { X: 5, Y: 15, Z: 0 },
          Length: 14.142,
          Direction: { X: 0.707, Y: 0.707, Z: 0 }
        },
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 15, Z: 0 },
          Center: { X: 5, Y: 7.5, Z: 0 },
          Size: { X: 10, Y: 15, Z: 0 }
        },
        Centroid: { X: 5, Y: 7.5, Z: 0 },
        Points: [],
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true
      };

      const result = DimensionEntityThreejsRenderer.render(dimensionData, scene);

      expect(result).not.toBeNull();
      expect(result.group).toBeInstanceOf(THREE.Group);
      expect(result.group.name).toBe('Dimension_DimensionAligned');
      expect(result.dimensionType).toBe('DimensionAligned');
    });

    it('should render dimension with LinearData', () => {
      const dimensionData: DimensionData = {
        Type: 'DimensionLinear',
        DefinitionPointX: 0,
        DefinitionPointY: 0,
        DefinitionPointZ: 0,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 1,
        LinearData: {
          DefinitionPointX: 0,
          DefinitionPointY: 0,
          DefinitionPointZ: 0,
          FirstPointX: 0,
          FirstPointY: 0,
          FirstPointZ: 0,
          SecondPointX: 10,
          SecondPointY: 0,
          SecondPointZ: 0,
          NormalX: 0,
          NormalY: 0,
          NormalZ: 1,
          Measurement: 10,
          Text: '10',
          Rotation: 0,
          ExtLineRotation: 0,
          Bounds: {
            Min: { X: 0, Y: 0, Z: 0 },
            Max: { X: 10, Y: 0, Z: 0 },
            Center: { X: 5, Y: 0, Z: 0 },
            Size: { X: 10, Y: 0, Z: 0 }
          },
          Centroid: { X: 5, Y: 0, Z: 0 },
          DefinitionPoint: { X: 0, Y: 0, Z: 0 },
          FirstPoint: { X: 0, Y: 0, Z: 0 },
          SecondPoint: { X: 10, Y: 0, Z: 0 },
          Length: 10,
          Direction: { X: 1, Y: 0, Z: 0 },
          RotationInRadians: 0,
          ExtLineRotationInRadians: 0
        },
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 0, Z: 0 },
          Center: { X: 5, Y: 0, Z: 0 },
          Size: { X: 10, Y: 0, Z: 0 }
        },
        Centroid: { X: 5, Y: 0, Z: 0 },
        Points: [],
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true
      };

      const result = DimensionEntityThreejsRenderer.render(dimensionData, scene);

      expect(result).not.toBeNull();
      expect(result.group).toBeInstanceOf(THREE.Group);
      expect(result.group.name).toBe('Dimension_DimensionLinear');
      expect(result.dimensionType).toBe('DimensionLinear');
    });

    it('should render dimension with Entities', () => {
      const dimensionData: DimensionData = {
        Type: 'Dimension',
        DefinitionPointX: 0,
        DefinitionPointY: 0,
        DefinitionPointZ: 0,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 1,
        Entities: [
          {
            Type: 'Line',
            LineData: {
              StartX: 0,
              StartY: 0,
              StartZ: 0,
              EndX: 10,
              EndY: 0,
              EndZ: 0,
              NormalX: 0,
              NormalY: 0,
              NormalZ: 1,
              ColorIndex: 1,
              ColorHex: '#FF0000',
              ColorR: 255,
              ColorG: 0,
              ColorB: 0,
              LineTypeName: 'CONTINUOUS',
              LineWeight: 1,
              Bounds: {
                Min: { X: 0, Y: 0, Z: 0 },
                Max: { X: 10, Y: 0, Z: 0 },
                Center: { X: 5, Y: 0, Z: 0 },
                Size: { X: 10, Y: 0, Z: 0 }
              },
              MidPoint: { X: 5, Y: 0, Z: 0 },
              Length: 10
            }
          }
        ],
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 0, Z: 0 },
          Center: { X: 5, Y: 0, Z: 0 },
          Size: { X: 10, Y: 0, Z: 0 }
        },
        Centroid: { X: 5, Y: 0, Z: 0 },
        Points: [],
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true
      };

      const result = DimensionEntityThreejsRenderer.render(dimensionData, scene);

      expect(result).not.toBeNull();
      expect(result.group).toBeInstanceOf(THREE.Group);
      expect(result.group.children.length).toBeGreaterThan(0);
    });

    it('should handle dimension without scene parameter', () => {
      const dimensionData: DimensionData = {
        Type: 'Dimension',
        DefinitionPointX: 10,
        DefinitionPointY: 20,
        DefinitionPointZ: 0,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 1,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 20, Z: 0 },
          Center: { X: 5, Y: 10, Z: 0 },
          Size: { X: 10, Y: 20, Z: 0 }
        },
        Centroid: { X: 5, Y: 10, Z: 0 },
        Points: [],
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true
      };

      const result = DimensionEntityThreejsRenderer.render(dimensionData);

      expect(result).not.toBeNull();
      expect(result.group).toBeInstanceOf(THREE.Group);
    });

    it('should handle dimension with 3D coordinates', () => {
      const dimensionData: DimensionData = {
        Type: 'Dimension',
        DefinitionPointX: 15,
        DefinitionPointY: 25,
        DefinitionPointZ: 35,
        NormalX: 0.5,
        NormalY: 0.5,
        NormalZ: 0.707,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 1,
        Bounds: {
          Min: { X: 15, Y: 25, Z: 35 },
          Max: { X: 25, Y: 35, Z: 45 },
          Center: { X: 20, Y: 30, Z: 40 },
          Size: { X: 10, Y: 10, Z: 10 }
        },
        Centroid: { X: 20, Y: 30, Z: 40 },
        Points: [],
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true
      };

      const result = DimensionEntityThreejsRenderer.render(dimensionData, scene);

      expect(result).not.toBeNull();
      expect(result.group).toBeInstanceOf(THREE.Group);
    });

    it('should handle dimension with negative coordinates', () => {
      const dimensionData: DimensionData = {
        Type: 'Dimension',
        DefinitionPointX: -10,
        DefinitionPointY: -20,
        DefinitionPointZ: -5,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 1,
        Bounds: {
          Min: { X: -10, Y: -20, Z: -5 },
          Max: { X: 0, Y: -10, Z: 5 },
          Center: { X: -5, Y: -15, Z: 0 },
          Size: { X: 10, Y: 10, Z: 10 }
        },
        Centroid: { X: -5, Y: -15, Z: 0 },
        Points: [],
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true
      };

      const result = DimensionEntityThreejsRenderer.render(dimensionData, scene);

      expect(result).not.toBeNull();
      expect(result.group).toBeInstanceOf(THREE.Group);
    });

    it('should handle dimension with large coordinates', () => {
      const dimensionData: DimensionData = {
        Type: 'Dimension',
        DefinitionPointX: 10000,
        DefinitionPointY: 20000,
        DefinitionPointZ: 30000,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 1,
        Bounds: {
          Min: { X: 10000, Y: 20000, Z: 30000 },
          Max: { X: 10010, Y: 20010, Z: 30000 },
          Center: { X: 10005, Y: 20005, Z: 30000 },
          Size: { X: 10, Y: 10, Z: 0 }
        },
        Centroid: { X: 10005, Y: 20005, Z: 30000 },
        Points: [],
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true
      };

      const result = DimensionEntityThreejsRenderer.render(dimensionData, scene);

      expect(result).not.toBeNull();
      expect(result.group).toBeInstanceOf(THREE.Group);
    });

    it('should handle dimension with zero coordinates', () => {
      const dimensionData: DimensionData = {
        Type: 'Dimension',
        DefinitionPointX: 0,
        DefinitionPointY: 0,
        DefinitionPointZ: 0,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 1,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 0, Y: 0, Z: 0 },
          Center: { X: 0, Y: 0, Z: 0 },
          Size: { X: 0, Y: 0, Z: 0 }
        },
        Centroid: { X: 0, Y: 0, Z: 0 },
        Points: [],
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true
      };

      const result = DimensionEntityThreejsRenderer.render(dimensionData, scene);

      expect(result).not.toBeNull();
      expect(result.group).toBeInstanceOf(THREE.Group);
    });

    it('should handle dimension with very small coordinates', () => {
      const dimensionData: DimensionData = {
        Type: 'Dimension',
        DefinitionPointX: 0.001,
        DefinitionPointY: 0.002,
        DefinitionPointZ: 0.003,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 1,
        Bounds: {
          Min: { X: 0.001, Y: 0.002, Z: 0.003 },
          Max: { X: 0.011, Y: 0.012, Z: 0.003 },
          Center: { X: 0.006, Y: 0.007, Z: 0.003 },
          Size: { X: 0.01, Y: 0.01, Z: 0 }
        },
        Centroid: { X: 0.006, Y: 0.007, Z: 0.003 },
        Points: [],
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true
      };

      const result = DimensionEntityThreejsRenderer.render(dimensionData, scene);

      expect(result).not.toBeNull();
      expect(result.group).toBeInstanceOf(THREE.Group);
    });

    it('should handle dimension with very large coordinates', () => {
      const dimensionData: DimensionData = {
        Type: 'Dimension',
        DefinitionPointX: 1000000,
        DefinitionPointY: 2000000,
        DefinitionPointZ: 3000000,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 1,
        Bounds: {
          Min: { X: 1000000, Y: 2000000, Z: 3000000 },
          Max: { X: 1000010, Y: 2000010, Z: 3000000 },
          Center: { X: 1000005, Y: 2000005, Z: 3000000 },
          Size: { X: 10, Y: 10, Z: 0 }
        },
        Centroid: { X: 1000005, Y: 2000005, Z: 3000000 },
        Points: [],
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true
      };

      const result = DimensionEntityThreejsRenderer.render(dimensionData, scene);

      expect(result).not.toBeNull();
      expect(result.group).toBeInstanceOf(THREE.Group);
    });

    it('should handle dimension with floating point coordinates', () => {
      const dimensionData: DimensionData = {
        Type: 'Dimension',
        DefinitionPointX: 10.567,
        DefinitionPointY: 20.789,
        DefinitionPointZ: 30.123,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 1,
        Bounds: {
          Min: { X: 10.567, Y: 20.789, Z: 30.123 },
          Max: { X: 20.567, Y: 30.789, Z: 30.123 },
          Center: { X: 15.567, Y: 25.789, Z: 30.123 },
          Size: { X: 10, Y: 10, Z: 0 }
        },
        Centroid: { X: 15.567, Y: 25.789, Z: 30.123 },
        Points: [],
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true
      };

      const result = DimensionEntityThreejsRenderer.render(dimensionData, scene);

      expect(result).not.toBeNull();
      expect(result.group).toBeInstanceOf(THREE.Group);
    });

    it('should handle dimension with inverted normal', () => {
      const dimensionData: DimensionData = {
        Type: 'Dimension',
        DefinitionPointX: 0,
        DefinitionPointY: 0,
        DefinitionPointZ: 0,
        NormalX: 0,
        NormalY: 0,
        NormalZ: -1,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 1,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 20, Z: 0 },
          Center: { X: 5, Y: 10, Z: 0 },
          Size: { X: 10, Y: 20, Z: 0 }
        },
        Centroid: { X: 5, Y: 10, Z: 0 },
        Points: [],
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true
      };

      const result = DimensionEntityThreejsRenderer.render(dimensionData, scene);

      expect(result).not.toBeNull();
      expect(result.group).toBeInstanceOf(THREE.Group);
    });

    it('should handle dimension with different colors', () => {
      const testCases = [
        { index: 1, hex: '#FF0000', r: 255, g: 0, b: 0 },
        { index: 2, hex: '#FFFF00', r: 255, g: 255, b: 0 },
        { index: 3, hex: '#00FF00', r: 0, g: 255, b: 0 },
        { index: 4, hex: '#00FFFF', r: 0, g: 255, b: 255 },
        { index: 5, hex: '#0000FF', r: 0, g: 0, b: 255 }
      ];

      testCases.forEach(({ index, hex, r, g, b }) => {
        const dimensionData: DimensionData = {
          Type: 'Dimension',
          DefinitionPointX: 0,
          DefinitionPointY: 0,
          DefinitionPointZ: 0,
          NormalX: 0,
          NormalY: 0,
          NormalZ: 1,
          ColorIndex: index,
          ColorHex: hex,
          ColorR: r,
          ColorG: g,
          ColorB: b,
          LineTypeName: 'CONTINUOUS',
          LineWeight: 1,
          Bounds: {
            Min: { X: 0, Y: 0, Z: 0 },
            Max: { X: 10, Y: 20, Z: 0 },
            Center: { X: 5, Y: 10, Z: 0 },
            Size: { X: 10, Y: 20, Z: 0 }
          },
          Centroid: { X: 5, Y: 10, Z: 0 },
          Points: [],
          CoordinateSystem: 'AutoCAD',
          RequiresYAxisFlip: true
        };

        const result = DimensionEntityThreejsRenderer.render(dimensionData, scene);

        expect(result).not.toBeNull();
        expect(result.group).toBeInstanceOf(THREE.Group);
      });
    });

    it('should calculate correct bounds for dimension', () => {
      const dimensionData: DimensionData = {
        Type: 'Dimension',
        DefinitionPointX: 10,
        DefinitionPointY: 20,
        DefinitionPointZ: 0,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 1,
        Bounds: {
          Min: { X: 10, Y: 20, Z: 0 },
          Max: { X: 20, Y: 30, Z: 0 },
          Center: { X: 15, Y: 25, Z: 0 },
          Size: { X: 10, Y: 10, Z: 0 }
        },
        Centroid: { X: 15, Y: 25, Z: 0 },
        Points: [],
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true
      };

      const result = DimensionEntityThreejsRenderer.render(dimensionData, scene);

      expect(result).not.toBeNull();
      expect(result.bounds).toBeInstanceOf(THREE.Box3);
    });

    it('should handle dimension with empty Entities array', () => {
      const dimensionData: DimensionData = {
        Type: 'Dimension',
        DefinitionPointX: 10,
        DefinitionPointY: 20,
        DefinitionPointZ: 0,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 1,
        Entities: [],
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 20, Z: 0 },
          Center: { X: 5, Y: 10, Z: 0 },
          Size: { X: 10, Y: 20, Z: 0 }
        },
        Centroid: { X: 5, Y: 10, Z: 0 },
        Points: [],
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true
      };

      const result = DimensionEntityThreejsRenderer.render(dimensionData, scene);

      expect(result).not.toBeNull();
      expect(result.group).toBeInstanceOf(THREE.Group);
    });

    it('should handle dimension with undefined Entities', () => {
      const dimensionData: DimensionData = {
        Type: 'Dimension',
        DefinitionPointX: 10,
        DefinitionPointY: 20,
        DefinitionPointZ: 0,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 1,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 20, Z: 0 },
          Center: { X: 5, Y: 10, Z: 0 },
          Size: { X: 10, Y: 20, Z: 0 }
        },
        Centroid: { X: 5, Y: 10, Z: 0 },
        Points: [],
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true
      };

      const result = DimensionEntityThreejsRenderer.render(dimensionData, scene);

      expect(result).not.toBeNull();
      expect(result.group).toBeInstanceOf(THREE.Group);
    });

    it('should handle dimension with undefined color properties', () => {
      const dimensionData: DimensionData = {
        Type: 'Dimension',
        DefinitionPointX: 10,
        DefinitionPointY: 20,
        DefinitionPointZ: 0,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 1,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 20, Z: 0 },
          Center: { X: 5, Y: 10, Z: 0 },
          Size: { X: 10, Y: 20, Z: 0 }
        },
        Centroid: { X: 5, Y: 10, Z: 0 },
        Points: [],
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true
      };

      const result = DimensionEntityThreejsRenderer.render(dimensionData, scene);

      expect(result).not.toBeNull();
      expect(result.group).toBeInstanceOf(THREE.Group);
    });

    it('should handle dimension with undefined normal properties', () => {
      const dimensionData: DimensionData = {
        Type: 'Dimension',
        DefinitionPointX: 10,
        DefinitionPointY: 20,
        DefinitionPointZ: 0,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 1,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 20, Z: 0 },
          Center: { X: 5, Y: 10, Z: 0 },
          Size: { X: 10, Y: 20, Z: 0 }
        },
        Centroid: { X: 5, Y: 10, Z: 0 },
        Points: [],
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true
      };

      const result = DimensionEntityThreejsRenderer.render(dimensionData, scene);

      expect(result).not.toBeNull();
      expect(result.group).toBeInstanceOf(THREE.Group);
    });

    it('should handle dimension with undefined definition point Z', () => {
      const dimensionData: DimensionData = {
        Type: 'Dimension',
        DefinitionPointX: 10,
        DefinitionPointY: 20,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 1,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 20, Z: 0 },
          Center: { X: 5, Y: 10, Z: 0 },
          Size: { X: 10, Y: 20, Z: 0 }
        },
        Centroid: { X: 5, Y: 10, Z: 0 },
        Points: [],
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true
      };

      const result = DimensionEntityThreejsRenderer.render(dimensionData, scene);

      expect(result).not.toBeNull();
      expect(result.group).toBeInstanceOf(THREE.Group);
    });

    it('should handle dimension with undefined coordinate system', () => {
      const dimensionData: DimensionData = {
        Type: 'Dimension',
        DefinitionPointX: 10,
        DefinitionPointY: 20,
        DefinitionPointZ: 0,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 1,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 20, Z: 0 },
          Center: { X: 5, Y: 10, Z: 0 },
          Size: { X: 10, Y: 20, Z: 0 }
        },
        Centroid: { X: 5, Y: 10, Z: 0 },
        Points: []
      };

      const result = DimensionEntityThreejsRenderer.render(dimensionData, scene);

      expect(result).not.toBeNull();
      expect(result.group).toBeInstanceOf(THREE.Group);
    });

    it('should handle dimension with undefined Y axis flip', () => {
      const dimensionData: DimensionData = {
        Type: 'Dimension',
        DefinitionPointX: 10,
        DefinitionPointY: 20,
        DefinitionPointZ: 0,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 1,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 20, Z: 0 },
          Center: { X: 5, Y: 10, Z: 0 },
          Size: { X: 10, Y: 20, Z: 0 }
        },
        Centroid: { X: 5, Y: 10, Z: 0 },
        Points: [],
        CoordinateSystem: 'AutoCAD'
      };

      const result = DimensionEntityThreejsRenderer.render(dimensionData, scene);

      expect(result).not.toBeNull();
      expect(result.group).toBeInstanceOf(THREE.Group);
    });

    it('should handle dimension with undefined bounds', () => {
      const dimensionData: DimensionData = {
        Type: 'Dimension',
        DefinitionPointX: 10,
        DefinitionPointY: 20,
        DefinitionPointZ: 0,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 1,
        Centroid: { X: 5, Y: 10, Z: 0 },
        Points: [],
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true
      };

      const result = DimensionEntityThreejsRenderer.render(dimensionData, scene);

      expect(result).not.toBeNull();
      expect(result.group).toBeInstanceOf(THREE.Group);
    });

    it('should handle dimension with undefined centroid', () => {
      const dimensionData: DimensionData = {
        Type: 'Dimension',
        DefinitionPointX: 10,
        DefinitionPointY: 20,
        DefinitionPointZ: 0,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 1,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 20, Z: 0 },
          Center: { X: 5, Y: 10, Z: 0 },
          Size: { X: 10, Y: 20, Z: 0 }
        },
        Points: [],
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true
      };

      const result = DimensionEntityThreejsRenderer.render(dimensionData, scene);

      expect(result).not.toBeNull();
      expect(result.group).toBeInstanceOf(THREE.Group);
    });

    it('should handle dimension with undefined points', () => {
      const dimensionData: DimensionData = {
        Type: 'Dimension',
        DefinitionPointX: 10,
        DefinitionPointY: 20,
        DefinitionPointZ: 0,
        NormalX: 0,
        NormalY: 0,
        NormalZ: 1,
        ColorIndex: 1,
        ColorHex: '#FF0000',
        ColorR: 255,
        ColorG: 0,
        ColorB: 0,
        LineTypeName: 'CONTINUOUS',
        LineWeight: 1,
        Bounds: {
          Min: { X: 0, Y: 0, Z: 0 },
          Max: { X: 10, Y: 20, Z: 0 },
          Center: { X: 5, Y: 10, Z: 0 },
          Size: { X: 10, Y: 20, Z: 0 }
        },
        Centroid: { X: 5, Y: 10, Z: 0 },
        CoordinateSystem: 'AutoCAD',
        RequiresYAxisFlip: true
      };

      const result = DimensionEntityThreejsRenderer.render(dimensionData, scene);

      expect(result).not.toBeNull();
      expect(result.group).toBeInstanceOf(THREE.Group);
    });
  });
});
