import * as THREE from 'three'
import { PathGeometry, PathPointList, PathTubeGeometry } from '/path/build/three.path.module.js';

let texturePath: THREE.Texture;
function initPath(points: THREE.Vector3[],renderer: THREE.WebGLRenderer,scene: THREE.Scene) {

//points 路径点数组变为字符串
let pointsStr = points.map(point => `${point.x},${point.y},${point.z}`).join(';');
// //pointsStr 路径点字符串变为路径点数组
// let pointsArr = pointsStr.split(';').map(point => {
// 	let arr = point.split(',');
// 	return new THREE.Vector3(parseFloat(arr[0]), parseFloat(arr[1]), parseFloat(arr[2]));
// });

	let up = new THREE.Vector3(0, 1, 0);
	// create PathPointList
	let pathPointList = new PathPointList();
	pathPointList.set(points, 1, 0, up);

	// create geometry
	let geometry = new PathGeometry();
	geometry.update(pathPointList, {
		width: 0.5,
		arrow: false,
		progress: 1,
	});

	texturePath = new THREE.TextureLoader().load('./images/path_007_18.png', function (texture) {
		texture.wrapS = texture.wrapT = THREE.RepeatWrapping;
		texture.anisotropy = renderer.capabilities.getMaxAnisotropy();
	});

	let material = new THREE.MeshPhongMaterial({
		color: 0x00ff00,
		depthTest: false,
		depthWrite: true,
		transparent: true,
		side: THREE.DoubleSide,
		opacity: 0.9,
	});
	material.map = texturePath;
	material.needsUpdate = true;

	let mesh = new THREE.Mesh(geometry, material);
	mesh.userData.type = 'pash';
	mesh.userData.id = pointsStr;
	scene.add(mesh);
}

//删除路径
function deletePath(points: THREE.Vector3[],renderer: THREE.WebGLRenderer,scene: THREE.Scene) {
	let pointsStr = points.map(point => `${point.x},${point.y},${point.z}`).join(';');
	let mesh = scene.children.find(child => child.userData.type === 'pash' && child.userData.id === pointsStr);
	if (mesh) {
		scene.remove(mesh);
	}
}

//导出路径字符串
function exportPath(points: THREE.Vector3[]) {
	let pointsStr = points.map(point => `${point.x},${point.y},${point.z}`).join(';');
	return pointsStr;
}
//导出路径点数组
function exportPoints(pointsStr: string) {
	let pointsArr = pointsStr.split(';').map(point => {
		let arr = point.split(',');
		return new THREE.Vector3(parseFloat(arr[0]), parseFloat(arr[1]), parseFloat(arr[2]));
	});
	return pointsArr;
}

//导出上面四个方法
export {
	initPath,
	deletePath,
	exportPath,
    exportPoints
}