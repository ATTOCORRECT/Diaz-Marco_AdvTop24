extends MeshInstance3D
class_name Chunk

var noise
var chunk_position
var chunk_size
var grid_size
var should_remove = true

var mesh_verts : PackedVector3Array
var mesh_uvs : PackedVector2Array
var mesh_normals : PackedVector3Array
var mesh_indexes : PackedInt32Array
var mesh_colors : PackedColorArray

const TRIANGULATION_TABLE = [
	[-1],
	[ 0, 8, 3, -1],
	[ 0, 1, 9, -1],
	[ 1, 8, 3, 9, 8, 1, -1],
	[ 1, 2, 10, -1],
	[ 0, 8, 3, 1, 2, 10, -1],
	[ 9, 2, 10, 0, 2, 9, -1],
	[ 2, 8, 3, 2, 10, 8, 10, 9, 8, -1],
	[ 3, 11, 2, -1],
	[ 0, 11, 2, 8, 11, 0, -1],
	[ 1, 9, 0, 2, 3, 11, -1],
	[ 1, 11, 2, 1, 9, 11, 9, 8, 11, -1],
	[ 3, 10, 1, 11, 10, 3, -1],
	[ 0, 10, 1, 0, 8, 10, 8, 11, 10, -1],
	[ 3, 9, 0, 3, 11, 9, 11, 10, 9, -1],
	[ 9, 8, 10, 10, 8, 11, -1],
	[ 4, 7, 8, -1],
	[ 4, 3, 0, 7, 3, 4, -1],
	[ 0, 1, 9, 8, 4, 7, -1],
	[ 4, 1, 9, 4, 7, 1, 7, 3, 1, -1],
	[ 1, 2, 10, 8, 4, 7, -1],
	[ 3, 4, 7, 3, 0, 4, 1, 2, 10, -1],
	[ 9, 2, 10, 9, 0, 2, 8, 4, 7, -1],
	[ 2, 10, 9, 2, 9, 7, 2, 7, 3, 7, 9, 4, -1],
	[ 8, 4, 7, 3, 11, 2, -1],
	[ 11, 4, 7, 11, 2, 4, 2, 0, 4, -1],
	[ 9, 0, 1, 8, 4, 7, 2, 3, 11, -1],
	[ 4, 7, 11, 9, 4, 11, 9, 11, 2, 9, 2, 1, -1],
	[ 3, 10, 1, 3, 11, 10, 7, 8, 4, -1],
	[ 1, 11, 10, 1, 4, 11, 1, 0, 4, 7, 11, 4, -1],
	[ 4, 7, 8, 9, 0, 11, 9, 11, 10, 11, 0, 3, -1],
	[ 4, 7, 11, 4, 11, 9, 9, 11, 10, -1],
	[ 9, 5, 4, -1],
	[ 9, 5, 4, 0, 8, 3, -1],
	[ 0, 5, 4, 1, 5, 0, -1],
	[ 8, 5, 4, 8, 3, 5, 3, 1, 5, -1],
	[ 1, 2, 10, 9, 5, 4, -1],
	[ 3, 0, 8, 1, 2, 10, 4, 9, 5, -1],
	[ 5, 2, 10, 5, 4, 2, 4, 0, 2, -1],
	[ 2, 10, 5, 3, 2, 5, 3, 5, 4, 3, 4, 8, -1],
	[ 9, 5, 4, 2, 3, 11, -1],
	[ 0, 11, 2, 0, 8, 11, 4, 9, 5, -1],
	[ 0, 5, 4, 0, 1, 5, 2, 3, 11, -1],
	[ 2, 1, 5, 2, 5, 8, 2, 8, 11, 4, 8, 5, -1],
	[ 10, 3, 11, 10, 1, 3, 9, 5, 4, -1],
	[ 4, 9, 5, 0, 8, 1, 8, 10, 1, 8, 11, 10, -1],
	[ 5, 4, 0, 5, 0, 11, 5, 11, 10, 11, 0, 3, -1],
	[ 5, 4, 8, 5, 8, 10, 10, 8, 11, -1],
	[ 9, 7, 8, 5, 7, 9, -1],
	[ 9, 3, 0, 9, 5, 3, 5, 7, 3, -1],
	[ 0, 7, 8, 0, 1, 7, 1, 5, 7, -1],
	[ 1, 5, 3, 3, 5, 7, -1],
	[ 9, 7, 8, 9, 5, 7, 10, 1, 2, -1],
	[ 10, 1, 2, 9, 5, 0, 5, 3, 0, 5, 7, 3, -1],
	[ 8, 0, 2, 8, 2, 5, 8, 5, 7, 10, 5, 2, -1],
	[ 2, 10, 5, 2, 5, 3, 3, 5, 7, -1],
	[ 7, 9, 5, 7, 8, 9, 3, 11, 2, -1],
	[ 9, 5, 7, 9, 7, 2, 9, 2, 0, 2, 7, 11, -1],
	[ 2, 3, 11, 0, 1, 8, 1, 7, 8, 1, 5, 7, -1],
	[ 11, 2, 1, 11, 1, 7, 7, 1, 5, -1],
	[ 9, 5, 8, 8, 5, 7, 10, 1, 3, 10, 3, 11, -1],
	[ 5, 7, 0, 5, 0, 9, 7, 11, 0, 1, 0, 10, 11, 10, 0, -1 ],
	[ 11, 10, 0, 11, 0, 3, 10, 5, 0, 8, 0, 7, 5, 7, 0, -1 ],
	[ 11, 10, 5, 7, 11, 5, -1],
	[ 10, 6, 5, -1],
	[ 0, 8, 3, 5, 10, 6, -1],
	[ 9, 0, 1, 5, 10, 6, -1],
	[ 1, 8, 3, 1, 9, 8, 5, 10, 6, -1],
	[ 1, 6, 5, 2, 6, 1, -1],
	[ 1, 6, 5, 1, 2, 6, 3, 0, 8, -1],
	[ 9, 6, 5, 9, 0, 6, 0, 2, 6, -1],
	[ 5, 9, 8, 5, 8, 2, 5, 2, 6, 3, 2, 8, -1],
	[ 2, 3, 11, 10, 6, 5, -1],
	[ 11, 0, 8, 11, 2, 0, 10, 6, 5, -1],
	[ 0, 1, 9, 2, 3, 11, 5, 10, 6, -1],
	[ 5, 10, 6, 1, 9, 2, 9, 11, 2, 9, 8, 11, -1],
	[ 6, 3, 11, 6, 5, 3, 5, 1, 3, -1],
	[ 0, 8, 11, 0, 11, 5, 0, 5, 1, 5, 11, 6, -1],
	[ 3, 11, 6, 0, 3, 6, 0, 6, 5, 0, 5, 9, -1],
	[ 6, 5, 9, 6, 9, 11, 11, 9, 8, -1],
	[ 5, 10, 6, 4, 7, 8, -1],
	[ 4, 3, 0, 4, 7, 3, 6, 5, 10, -1],
	[ 1, 9, 0, 5, 10, 6, 8, 4, 7, -1],
	[ 10, 6, 5, 1, 9, 7, 1, 7, 3, 7, 9, 4, -1],
	[ 6, 1, 2, 6, 5, 1, 4, 7, 8, -1],
	[ 1, 2, 5, 5, 2, 6, 3, 0, 4, 3, 4, 7, -1],
	[ 8, 4, 7, 9, 0, 5, 0, 6, 5, 0, 2, 6, -1],
	[ 7, 3, 9, 7, 9, 4, 3, 2, 9, 5, 9, 6, 2, 6, 9, -1 ],
	[ 3, 11, 2, 7, 8, 4, 10, 6, 5, -1],
	[ 5, 10, 6, 4, 7, 2, 4, 2, 0, 2, 7, 11, -1],
	[ 0, 1, 9, 4, 7, 8, 2, 3, 11, 5, 10, 6, -1],
	[ 9, 2, 1, 9, 11, 2, 9, 4, 11, 7, 11, 4, 5, 10, 6, -1 ],
	[ 8, 4, 7, 3, 11, 5, 3, 5, 1, 5, 11, 6, -1],
	[ 5, 1, 11, 5, 11, 6, 1, 0, 11, 7, 11, 4, 0, 4, 11, -1 ],
	[ 0, 5, 9, 0, 6, 5, 0, 3, 6, 11, 6, 3, 8, 4, 7, -1 ],
	[ 6, 5, 9, 6, 9, 11, 4, 7, 9, 7, 11, 9, -1],
	[ 10, 4, 9, 6, 4, 10, -1],
	[ 4, 10, 6, 4, 9, 10, 0, 8, 3, -1],
	[ 10, 0, 1, 10, 6, 0, 6, 4, 0, -1],
	[ 8, 3, 1, 8, 1, 6, 8, 6, 4, 6, 1, 10, -1],
	[ 1, 4, 9, 1, 2, 4, 2, 6, 4, -1],
	[ 3, 0, 8, 1, 2, 9, 2, 4, 9, 2, 6, 4, -1],
	[ 0, 2, 4, 4, 2, 6, -1],
	[ 8, 3, 2, 8, 2, 4, 4, 2, 6, -1],
	[ 10, 4, 9, 10, 6, 4, 11, 2, 3, -1],
	[ 0, 8, 2, 2, 8, 11, 4, 9, 10, 4, 10, 6, -1],
	[ 3, 11, 2, 0, 1, 6, 0, 6, 4, 6, 1, 10, -1],
	[ 6, 4, 1, 6, 1, 10, 4, 8, 1, 2, 1, 11, 8, 11, 1, -1 ],
	[ 9, 6, 4, 9, 3, 6, 9, 1, 3, 11, 6, 3, -1],
	[ 8, 11, 1, 8, 1, 0, 11, 6, 1, 9, 1, 4, 6, 4, 1, -1 ],
	[ 3, 11, 6, 3, 6, 0, 0, 6, 4, -1],
	[ 6, 4, 8, 11, 6, 8, -1],
	[ 7, 10, 6, 7, 8, 10, 8, 9, 10, -1],
	[ 0, 7, 3, 0, 10, 7, 0, 9, 10, 6, 7, 10, -1],
	[ 10, 6, 7, 1, 10, 7, 1, 7, 8, 1, 8, 0, -1],
	[ 10, 6, 7, 10, 7, 1, 1, 7, 3, -1],
	[ 1, 2, 6, 1, 6, 8, 1, 8, 9, 8, 6, 7, -1],
	[ 2, 6, 9, 2, 9, 1, 6, 7, 9, 0, 9, 3, 7, 3, 9, -1 ],
	[ 7, 8, 0, 7, 0, 6, 6, 0, 2, -1],
	[ 7, 3, 2, 6, 7, 2, -1],
	[ 2, 3, 11, 10, 6, 8, 10, 8, 9, 8, 6, 7, -1],
	[ 2, 0, 7, 2, 7, 11, 0, 9, 7, 6, 7, 10, 9, 10, 7, -1 ],
	[ 1, 8, 0, 1, 7, 8, 1, 10, 7, 6, 7, 10, 2, 3, 11, -1 ],
	[ 11, 2, 1, 11, 1, 7, 10, 6, 1, 6, 7, 1, -1],
	[ 8, 9, 6, 8, 6, 7, 9, 1, 6, 11, 6, 3, 1, 3, 6, -1 ],
	[ 0, 9, 1, 11, 6, 7, -1],
	[ 7, 8, 0, 7, 0, 6, 3, 11, 0, 11, 6, 0, -1],
	[ 7, 11, 6, -1],
	[ 7, 6, 11, -1],
	[ 3, 0, 8, 11, 7, 6, -1],
	[ 0, 1, 9, 11, 7, 6, -1],
	[ 8, 1, 9, 8, 3, 1, 11, 7, 6, -1],
	[ 10, 1, 2, 6, 11, 7, -1],
	[ 1, 2, 10, 3, 0, 8, 6, 11, 7, -1],
	[ 2, 9, 0, 2, 10, 9, 6, 11, 7, -1],
	[ 6, 11, 7, 2, 10, 3, 10, 8, 3, 10, 9, 8, -1],
	[ 7, 2, 3, 6, 2, 7, -1],
	[ 7, 0, 8, 7, 6, 0, 6, 2, 0, -1],
	[ 2, 7, 6, 2, 3, 7, 0, 1, 9, -1],
	[ 1, 6, 2, 1, 8, 6, 1, 9, 8, 8, 7, 6, -1],
	[ 10, 7, 6, 10, 1, 7, 1, 3, 7, -1],
	[ 10, 7, 6, 1, 7, 10, 1, 8, 7, 1, 0, 8, -1],
	[ 0, 3, 7, 0, 7, 10, 0, 10, 9, 6, 10, 7, -1],
	[ 7, 6, 10, 7, 10, 8, 8, 10, 9, -1],
	[ 6, 8, 4, 11, 8, 6, -1],
	[ 3, 6, 11, 3, 0, 6, 0, 4, 6, -1],
	[ 8, 6, 11, 8, 4, 6, 9, 0, 1, -1],
	[ 9, 4, 6, 9, 6, 3, 9, 3, 1, 11, 3, 6, -1],
	[ 6, 8, 4, 6, 11, 8, 2, 10, 1, -1],
	[ 1, 2, 10, 3, 0, 11, 0, 6, 11, 0, 4, 6, -1],
	[ 4, 11, 8, 4, 6, 11, 0, 2, 9, 2, 10, 9, -1],
	[ 10, 9, 3, 10, 3, 2, 9, 4, 3, 11, 3, 6, 4, 6, 3, -1 ],
	[ 8, 2, 3, 8, 4, 2, 4, 6, 2, -1],
	[ 0, 4, 2, 4, 6, 2, -1],
	[ 1, 9, 0, 2, 3, 4, 2, 4, 6, 4, 3, 8, -1],
	[ 1, 9, 4, 1, 4, 2, 2, 4, 6, -1],
	[ 8, 1, 3, 8, 6, 1, 8, 4, 6, 6, 10, 1, -1],
	[ 10, 1, 0, 10, 0, 6, 6, 0, 4, -1],
	[ 4, 6, 3, 4, 3, 8, 6, 10, 3, 0, 3, 9, 10, 9, 3, -1 ],
	[ 10, 9, 4, 6, 10, 4, -1],
	[ 4, 9, 5, 7, 6, 11, -1],
	[ 0, 8, 3, 4, 9, 5, 11, 7, 6, -1],
	[ 5, 0, 1, 5, 4, 0, 7, 6, 11, -1],
	[ 11, 7, 6, 8, 3, 4, 3, 5, 4, 3, 1, 5, -1],
	[ 9, 5, 4, 10, 1, 2, 7, 6, 11, -1],
	[ 6, 11, 7, 1, 2, 10, 0, 8, 3, 4, 9, 5, -1],
	[ 7, 6, 11, 5, 4, 10, 4, 2, 10, 4, 0, 2, -1],
	[ 3, 4, 8, 3, 5, 4, 3, 2, 5, 10, 5, 2, 11, 7, 6, -1 ],
	[ 7, 2, 3, 7, 6, 2, 5, 4, 9, -1],
	[ 9, 5, 4, 0, 8, 6, 0, 6, 2, 6, 8, 7, -1],
	[ 3, 6, 2, 3, 7, 6, 1, 5, 0, 5, 4, 0, -1],
	[ 6, 2, 8, 6, 8, 7, 2, 1, 8, 4, 8, 5, 1, 5, 8, -1 ],
	[ 9, 5, 4, 10, 1, 6, 1, 7, 6, 1, 3, 7, -1],
	[ 1, 6, 10, 1, 7, 6, 1, 0, 7, 8, 7, 0, 9, 5, 4, -1 ],
	[ 4, 0, 10, 4, 10, 5, 0, 3, 10, 6, 10, 7, 3, 7, 10, -1 ],
	[ 7, 6, 10, 7, 10, 8, 5, 4, 10, 4, 8, 10, -1],
	[ 6, 9, 5, 6, 11, 9, 11, 8, 9, -1],
	[ 3, 6, 11, 0, 6, 3, 0, 5, 6, 0, 9, 5, -1],
	[ 0, 11, 8, 0, 5, 11, 0, 1, 5, 5, 6, 11, -1],
	[ 6, 11, 3, 6, 3, 5, 5, 3, 1, -1],
	[ 1, 2, 10, 9, 5, 11, 9, 11, 8, 11, 5, 6, -1],
	[ 0, 11, 3, 0, 6, 11, 0, 9, 6, 5, 6, 9, 1, 2, 10, -1 ],
	[ 11, 8, 5, 11, 5, 6, 8, 0, 5, 10, 5, 2, 0, 2, 5, -1 ],
	[ 6, 11, 3, 6, 3, 5, 2, 10, 3, 10, 5, 3, -1],
	[ 5, 8, 9, 5, 2, 8, 5, 6, 2, 3, 8, 2, -1],
	[ 9, 5, 6, 9, 6, 0, 0, 6, 2, -1],
	[ 1, 5, 8, 1, 8, 0, 5, 6, 8, 3, 8, 2, 6, 2, 8, -1 ],
	[ 1, 5, 6, 2, 1, 6, -1],
	[ 1, 3, 6, 1, 6, 10, 3, 8, 6, 5, 6, 9, 8, 9, 6, -1 ],
	[ 10, 1, 0, 10, 0, 6, 9, 5, 0, 5, 6, 0, -1],
	[ 0, 3, 8, 5, 6, 10, -1],
	[ 10, 5, 6, -1],
	[ 11, 5, 10, 7, 5, 11, -1],
	[ 11, 5, 10, 11, 7, 5, 8, 3, 0, -1],
	[ 5, 11, 7, 5, 10, 11, 1, 9, 0, -1],
	[ 10, 7, 5, 10, 11, 7, 9, 8, 1, 8, 3, 1, -1],
	[ 11, 1, 2, 11, 7, 1, 7, 5, 1, -1],
	[ 0, 8, 3, 1, 2, 7, 1, 7, 5, 7, 2, 11, -1],
	[ 9, 7, 5, 9, 2, 7, 9, 0, 2, 2, 11, 7, -1],
	[ 7, 5, 2, 7, 2, 11, 5, 9, 2, 3, 2, 8, 9, 8, 2, -1 ],
	[ 2, 5, 10, 2, 3, 5, 3, 7, 5, -1],
	[ 8, 2, 0, 8, 5, 2, 8, 7, 5, 10, 2, 5, -1],
	[ 9, 0, 1, 5, 10, 3, 5, 3, 7, 3, 10, 2, -1],
	[ 9, 8, 2, 9, 2, 1, 8, 7, 2, 10, 2, 5, 7, 5, 2, -1 ],
	[ 1, 3, 5, 3, 7, 5, -1],
	[ 0, 8, 7, 0, 7, 1, 1, 7, 5, -1],
	[ 9, 0, 3, 9, 3, 5, 5, 3, 7, -1],
	[ 9, 8, 7, 5, 9, 7, -1],
	[ 5, 8, 4, 5, 10, 8, 10, 11, 8, -1],
	[ 5, 0, 4, 5, 11, 0, 5, 10, 11, 11, 3, 0, -1],
	[ 0, 1, 9, 8, 4, 10, 8, 10, 11, 10, 4, 5, -1],
	[ 10, 11, 4, 10, 4, 5, 11, 3, 4, 9, 4, 1, 3, 1, 4, -1 ],
	[ 2, 5, 1, 2, 8, 5, 2, 11, 8, 4, 5, 8, -1],
	[ 0, 4, 11, 0, 11, 3, 4, 5, 11, 2, 11, 1, 5, 1, 11, -1 ],
	[ 0, 2, 5, 0, 5, 9, 2, 11, 5, 4, 5, 8, 11, 8, 5, -1 ],
	[ 9, 4, 5, 2, 11, 3, -1],
	[ 2, 5, 10, 3, 5, 2, 3, 4, 5, 3, 8, 4, -1],
	[ 5, 10, 2, 5, 2, 4, 4, 2, 0, -1],
	[ 3, 10, 2, 3, 5, 10, 3, 8, 5, 4, 5, 8, 0, 1, 9, -1 ],
	[ 5, 10, 2, 5, 2, 4, 1, 9, 2, 9, 4, 2, -1],
	[ 8, 4, 5, 8, 5, 3, 3, 5, 1, -1],
	[ 0, 4, 5, 1, 0, 5, -1],
	[ 8, 4, 5, 8, 5, 3, 9, 0, 5, 0, 3, 5, -1],
	[ 9, 4, 5, -1],
	[ 4, 11, 7, 4, 9, 11, 9, 10, 11, -1],
	[ 0, 8, 3, 4, 9, 7, 9, 11, 7, 9, 10, 11, -1],
	[ 1, 10, 11, 1, 11, 4, 1, 4, 0, 7, 4, 11, -1],
	[ 3, 1, 4, 3, 4, 8, 1, 10, 4, 7, 4, 11, 10, 11, 4, -1 ],
	[ 4, 11, 7, 9, 11, 4, 9, 2, 11, 9, 1, 2, -1],
	[ 9, 7, 4, 9, 11, 7, 9, 1, 11, 2, 11, 1, 0, 8, 3, -1 ],
	[ 11, 7, 4, 11, 4, 2, 2, 4, 0, -1],
	[ 11, 7, 4, 11, 4, 2, 8, 3, 4, 3, 2, 4, -1],
	[ 2, 9, 10, 2, 7, 9, 2, 3, 7, 7, 4, 9, -1],
	[ 9, 10, 7, 9, 7, 4, 10, 2, 7, 8, 7, 0, 2, 0, 7, -1 ],
	[ 3, 7, 10, 3, 10, 2, 7, 4, 10, 1, 10, 0, 4, 0, 10, -1 ],
	[ 1, 10, 2, 8, 7, 4, -1],
	[ 4, 9, 1, 4, 1, 7, 7, 1, 3, -1],
	[ 4, 9, 1, 4, 1, 7, 0, 8, 1, 8, 7, 1, -1],
	[ 4, 0, 3, 7, 4, 3, -1],
	[ 4, 8, 7, -1],
	[ 9, 10, 8, 10, 11, 8, -1],
	[ 3, 0, 9, 3, 9, 11, 11, 9, 10, -1],
	[ 0, 1, 10, 0, 10, 8, 8, 10, 11, -1],
	[ 3, 1, 10, 11, 3, 10, -1],
	[ 1, 2, 11, 1, 11, 9, 9, 11, 8, -1],
	[ 3, 0, 9, 3, 9, 11, 1, 2, 9, 2, 11, 9, -1],
	[ 0, 2, 11, 8, 0, 11, -1],
	[ 3, 2, 11, -1],
	[ 2, 3, 8, 2, 8, 10, 10, 8, 9, -1],
	[ 9, 10, 2, 0, 9, 2, -1],
	[ 2, 3, 8, 2, 8, 10, 0, 1, 8, 1, 10, 8, -1],
	[ 1, 10, 2, -1],
	[ 1, 3, 8, 9, 1, 8, -1],
	[ 0, 9, 1, -1],
	[ 0, 3, 8, -1],
	[-1]
]
const EDGE_TABLE = [
	[ 0, 1],
	[ 1, 2],
	[ 2, 3],
	[ 3, 0],
	[ 4, 5],
	[ 5, 6],
	[ 6, 7],
	[ 7, 4],
	[ 0, 4],
	[ 1, 5],
	[ 2, 6],
	[ 3, 7]
]

var terrain_density = {}
var cube_indexes = {}
var cube_positions = {}

const SURFACE_LEVEL = 0

func _init(in_noise, in_chunk_position, in_chunk_size):
	noise = in_noise
	chunk_position = in_chunk_position
	chunk_size = in_chunk_size
	grid_size = chunk_size + Vector3i.ONE

func _ready():
	position = chunk_size * chunk_position
	generate_chunk()

func generate_chunk():
	var surface_array = []
	surface_array.resize(Mesh.ARRAY_MAX)
	
	# PackedVector**Arrays for mesh construction
	mesh_verts   = PackedVector3Array()
	mesh_uvs     = PackedVector2Array()
	mesh_normals = PackedVector3Array()
	mesh_indexes = PackedInt32Array()
	
	var chunk_row    = chunk_size[0]
	var chunk_slice  = chunk_size[0] * chunk_size[1]
	var chunk_volume = chunk_size[0] * chunk_size[1] * chunk_size[2] 
	
	var grid_row    = grid_size[0]
	var grid_slice  = grid_size[0] * grid_size[1]
	var grid_volume = grid_size[0] * grid_size[1] * grid_size[2] 
	
	# Get Noise
	for i in grid_volume:
		# Coordinates
		var x =  i               % grid_size[0]
		var y = (i / grid_row  ) % grid_size[1]
		var z = (i / grid_slice) % grid_size[2]
		# Sample Noise
		terrain_density[i] = get_density(position + Vector3(x, y, z))
	
	# Generate triangles
	for i in chunk_volume:
		# Coordinates
		var x =  i                % chunk_size[0]
		var y = (i / chunk_row  ) % chunk_size[1]
		var z = (i / chunk_slice) % chunk_size[2]
		
		# Get positions for each vertex
		var cube_position = {}
		
		cube_position[0] = Vector3(x    , y    , z    ) # IMPROVE THIS
		cube_position[1] = Vector3(x + 1, y    , z    )
		cube_position[2] = Vector3(x + 1, y + 1, z    )
		cube_position[3] = Vector3(x    , y + 1, z    )
		cube_position[4] = Vector3(x    , y    , z + 1)
		cube_position[5] = Vector3(x + 1, y    , z + 1)
		cube_position[6] = Vector3(x + 1, y + 1, z + 1)
		cube_position[7] = Vector3(x    , y + 1, z + 1)
		
		# Get noise values for each vertex
		var ip = x + (y * grid_row) + (z * grid_slice)
		var cube_density = {}
		
		cube_density[0] = terrain_density[ip                            ]
		cube_density[1] = terrain_density[ip + 1                        ]
		cube_density[2] = terrain_density[ip + 1 + grid_row             ]
		cube_density[3] = terrain_density[ip     + grid_row             ]
		cube_density[4] = terrain_density[ip                + grid_slice]
		cube_density[5] = terrain_density[ip + 1            + grid_slice]
		cube_density[6] = terrain_density[ip + 1 + grid_row + grid_slice]
		cube_density[7] = terrain_density[ip     + grid_row + grid_slice]
		
		var triangulation_index = 0
		for j in 8:
			var point_density = cube_density[j]
			if point_density < SURFACE_LEVEL:
				triangulation_index |= 1<<j
		var triangulation = TRIANGULATION_TABLE[triangulation_index]
		
		var triangle_vertices = {}
		for j in triangulation.size() -1:
			var edge_index = triangulation[j]
			
			var vertex_index_A = EDGE_TABLE[edge_index][0]
			var vertex_index_B = EDGE_TABLE[edge_index][1]
			
			var vertex_position_A = cube_position[vertex_index_A]
			var vertex_position_B = cube_position[vertex_index_B]
			
			var vertex_density_A = cube_density[vertex_index_A]
			var vertex_density_B = cube_density[vertex_index_B]
			
			var bias = (SURFACE_LEVEL - vertex_density_A) / (vertex_density_B - vertex_density_A)
			
			triangle_vertices[j] = vertex_position_A.lerp(vertex_position_B, bias)
		
		append_triangles(triangle_vertices)

	# Return if there isnt any mesh to build
	if mesh_verts.size() == 0:
		return

	# Assign arrays to surface array
	surface_array[Mesh.ARRAY_VERTEX] = mesh_verts
	surface_array[Mesh.ARRAY_NORMAL] = mesh_normals
	surface_array[Mesh.ARRAY_INDEX]  = mesh_indexes
	surface_array[Mesh.ARRAY_COLOR]  = mesh_colors

	# Create mesh surface from mesh array.
	# No blendshapes, lods, or compression used.
	var arr_mesh = ArrayMesh.new()
	arr_mesh.add_surface_from_arrays(Mesh.PRIMITIVE_TRIANGLES, surface_array)
	mesh = arr_mesh
	var material = StandardMaterial3D.new()
	material.vertex_color_use_as_albedo = true
	set_surface_override_material(0, material)

	create_trimesh_collision()
	#var collision : StaticBody3D = get_child(0)
	#collision.set_collision_mask_value(2,true)

func _process(_delta: float) -> void:
	
	# chunk border
	DebugDraw3D.draw_box(position, Quaternion.IDENTITY, chunk_size, Color.WHITE)
	pass

func append_triangles(triangle_vertices):
	for i in triangle_vertices.size() / 3:
		var v1 : Vector3 = triangle_vertices[i * 3    ]
		var v2 : Vector3 = triangle_vertices[i * 3 + 1]
		var v3 : Vector3 = triangle_vertices[i * 3 + 2]
		
		append_triangle(v1, v2, v3)

func append_triangle(u: Vector3, v: Vector3, w: Vector3):
	var direction = (v - u).cross(w - u)
	var normal = direction.normalized()

	append_vertex(w, normal)
	append_vertex(v, normal)
	append_vertex(u, normal)
	append_color3(u)
	
func append_color3(v: Vector3):
	var y = (v.y + position.y)
	var R = (200 + y * 1 + randf() * 5) / 255.0
	var G = (60  - y * 2 + randf() * 5) / 255.0
	var B = (40  - y * 3 + randf() * 5) / 255.0
	
	var c = Color(R, G, B)
	
	mesh_colors.append(c)
	mesh_colors.append(c)
	mesh_colors.append(c)

func append_vertex(u: Vector3, n: Vector3):
	#var i = mesh_verts.find(u)
	#if  i != -1:
		#mesh_indexes.append(i)
	#else:
	mesh_verts.append(u)
	mesh_indexes.append(mesh_verts.size() - 1)
	mesh_normals.append(n)

func get_density(global_cell_position: Vector3):
	var x = global_cell_position.x
	var y = global_cell_position.y
	var z = global_cell_position.z
	
	var noise = noise.get_noise_3d(x, y, z)
	var temp = -((y) ** 2.0 / 2.0 ** 11.0) + 1/((y) + 0.01) + (int(y) % 4)/128.0 - 0.2
	var t = clamp(Vector2(x, z).length() / 32.0, 0, 1)
	var d = -sign(y - 4)
	return lerpf(d, noise + temp, t)
