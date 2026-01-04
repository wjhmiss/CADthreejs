/**
 * City Builder
 * @param {Object} params
 */
function City(params) {
  this._roadTexture = params.roadTexture || null;
  this._buildingMat = params.buildingMat || {
    color: 0x114499, //0x2194ce,
    transparent: true,
    opacity: 0.3,
    depthTest: true,
    depthWrite: false,
    blending: THREE.AdditiveBlending,
  };
  this.roadsGroup = this._createRoads(
    params.roadWidth || 2,
    params.roadOffset || 0.6
  );

  this.roads = this.roadsGroup.children;
  this.speeds = [];

  this._progress = 0;
}

City.prototype = Object.assign(City.prototype, {
  run: function (delta) {
    if (this._progress <= 1) {
      this._progress += 0.5 * delta;

      var that = this;
      this.roads.forEach(function (obj) {
        var geometry = obj.geometry;
        geometry._updateParam.progress = that._progress;
        geometry.update(geometry._pathPointList, geometry._updateParam);
        geometry.computeBoundingBox();
        geometry.computeBoundingSphere();
      });
    }

    this._roadTexture.offset.x -= delta * 0.02 * 24;
    this._roadTexture.repeat.x = 1 / 24;
  },
  _createRoadGeometries: function (roadWidth) {
    var width = roadWidth; // road width

    var points = [];
    points.push(new THREE.Vector3(10, 0, 10));
    points.push(new THREE.Vector3(10, 0, -10));
    points.push(new THREE.Vector3(-10, 0, -10));
    points.push(new THREE.Vector3(-10, 0, 10));

    var pathPointList = new THREE.PathPointList();
    pathPointList.set(points, 0, 0, new THREE.Vector3(0, 1, 0));

    var updateParam = {
      width: width,
      arrow: false,
      progress: 0,
    };

    var geometry = new THREE.PathGeometry({
      pathPointList: pathPointList,
      options: updateParam,
    });
    geometry._pathPointList = pathPointList;
    geometry._updateParam = updateParam;

    return geometry;
  },
  _createRoads: function (roadWidth) {
    var geometry = this._createRoadGeometries(roadWidth);
    var texture = this._roadTexture;

    var group = new THREE.Group();

    var material = new THREE.MeshBasicMaterial({
      depthWrite: false,
      depthTest: true,
      transparent: true,
      blending: THREE.AdditiveBlending,
      side: THREE.DoubleSide,
      map: texture,
    });
    var mesh = new THREE.Mesh(geometry, material);
    group.add(mesh);
    return group;
  },
});
