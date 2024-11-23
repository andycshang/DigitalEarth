/*
 * @Description: 地球自转效果
 * @Version: 1.0
 * @Author: Julian
 * @Date: 2022-02-25 15:14:20
 * @LastEditors: Julian
 * @LastEditTime: 2022-02-26 12:18:03
 */


/**
 * @description: 地球自转类
 */
  class GlobeRotate {
    constructor(viewer) {
        this._viewer = viewer;
    }

    _icrf() {
        if (this._viewer.scene.mode !== Cesium.SceneMode.SCENE3D) {
            // console.log("Not in 3D mode");
            return true;
        }
    
        let icrfToFixed = Cesium.Transforms.computeIcrfToFixedMatrix(this._viewer.clock.currentTime);
        if (icrfToFixed) {
            // console.log("ICRF executed");
    
            // 打印一些相机状态信息
            let camera = this._viewer.camera;
            // console.log("Camera position: ", camera.position);
        
            
            let offset = Cesium.Cartesian3.clone(camera.position);
            let transform = Cesium.Matrix4.fromRotationTranslation(icrfToFixed);
            camera.lookAtTransform(transform, offset);
        }
    }
    

    _bindEvent() {
        this._viewer.scene.postRender.addEventListener(this._icrf, this);
    }
    
    _unbindEvent() {
        this._viewer.scene.postRender.removeEventListener(this._icrf, this);
    }
    
    // 开始旋转
    start() {
        this._viewer.clock.shouldAnimate = true;
        this._unbindEvent();
        this._bindEvent();
        return this;
    }

    // 停止旋转
    stop() {
        this._unbindEvent();
        return this;
    }
}