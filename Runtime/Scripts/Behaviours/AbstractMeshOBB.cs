using nobnak.Gist.Extensions.AABB;
using nobnak.Gist.Intersection;
using nobnak.Gist.Primitive;
using UnityEngine;

namespace Recon.BehaviourSys {

	public abstract class AbstractMeshOBB : ConvexBuilder {
        public enum CoordinatesEnum { Skin = 0, Self, World }

        [SerializeField]
        protected CoordinatesEnum targetCoordinates;

        protected ConvexUpdator _convUp;
        protected OBB3 _obb;

        #region Abstract
        protected abstract Transform RootTransform();
        protected abstract Bounds LocalBounds();
        #endregion

        #region Unity
        protected virtual void OnDrawGizmos() {
			if (!isActiveAndEnabled)
                return;

            ConvUp.AssureUpdateConvex ();
			if (_obb != null)
				_obb.DrawGizmos ();
        }
        #endregion

        #region implemented abstract members of IConvex
        public override IConvex3Polytope GetConvexPolyhedron () {
            ConvUp.AssureUpdateConvex ();
            return _obb;
        }
        public override bool UpdateConvex () {
            return (_obb = CreateOBB()) != null;
        }
        #endregion

        public ConvexUpdator ConvUp {
            get { return (_convUp == null ? (_convUp = new ConvexUpdator (this)) : _convUp); }
        }

        OBB3 CreateOBB() {
            var rootBone = RootTransform();
            var localBounds = (FastBounds)LocalBounds();

            switch (targetCoordinates) {
            case CoordinatesEnum.World:
                return new OBB3 (rootBone.EncapsulateInWorldSpace (localBounds), Matrix4x4.identity);
            case CoordinatesEnum.Self:
                var rootToSelfMatrix = transform.worldToLocalMatrix * rootBone.localToWorldMatrix;
                return new OBB3 (localBounds.EncapsulateInTargetSpace (rootToSelfMatrix), 
                    transform.localToWorldMatrix);
            default:
                return OBB3.Create (rootBone, localBounds);
            }
        }
    }
}
