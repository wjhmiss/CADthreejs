export interface CadFile {
  id: string;
  name: string;
  size: number;
  uploadTime: string;
  status: 'uploaded' | 'parsing' | 'completed' | 'failed';
}

export interface CadParseResult {
  success: boolean;
  fileName: string;
  entityCount: number;
  data: string;
}

export interface CadEntity {
  type: string;
  handle: string;
  layer: string;
  color: string;
  data: any;
}

export interface ApiResponse<T> {
  success: boolean;
  data: T;
  message?: string;
  error?: string;
}

export interface UploadRequest {
  file: File;
  onProgress?: (progress: number) => void;
}

export interface Point3D {
  x: number;
  y: number;
  z: number;
}

export interface BoundsData {
  min: Point3D;
  max: Point3D;
}

export interface EntityData {
  type: string;
  handle: string;
  layer?: string;
  color?: string;
  lineWeight?: number;
  bounds?: BoundsData;
  [key: string]: any;
}
