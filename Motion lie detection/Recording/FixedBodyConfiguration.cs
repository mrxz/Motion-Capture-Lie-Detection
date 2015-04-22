using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Motion_lie_detection
{
    /**
	 * Simple body configuration that provides a static mapping and configuration of the body.
	 * This mapping is based on the default segment layout of XSens hardware.
	 */
    public class FixedBodyConfiguration : BodyConfiguration
    {

        public FixedBodyConfiguration()
            : base()
        {
            // Spine
            BodyNode pelvis = new BodyNode(1, "Pelvis");
            BodyNode l5 = new BodyNode(2, "L5");
            BodyNode l3 = new BodyNode(3, "L3");
            BodyNode t12 = new BodyNode(4, "T12");
            BodyNode t8 = new BodyNode(5, "T8");
            BodyNode neck = new BodyNode(6, "Neck");
            BodyNode head = new BodyNode(7, "Head");
            l5.setRoot(pelvis);
            l3.setRoot(pelvis);
            t12.setRoot(pelvis);
            t8.setRoot(pelvis);
            neck.setRoot(pelvis);
            head.setRoot(neck);

            pelvis.addNeighbour(l5);
            l5.addNeighbour(l3);
            l3.addNeighbour(t12);
            t12.addNeighbour(t8);
            t8.addNeighbour(neck);
            neck.addNeighbour(head);


            // Left leg.
            {
                BodyNode lT = new BodyNode(23, "LeftToe");
                BodyNode lF = new BodyNode(22, "LeftFoot");
                BodyNode lL = new BodyNode(21, "LeftLowerLeg");
                BodyNode lU = new BodyNode(20, "LeftUpperLeg");
                lT.setRoot(lU);
                lF.setRoot(lU);
                lL.setRoot(lU);
                lU.setRoot(lU);

                lF.addNeighbour(lT);
                lL.addNeighbour(lF);
                lU.addNeighbour(lL);
                pelvis.addNeighbour(lU);
            }

            // Right leg.
            {
                BodyNode rT = new BodyNode(19, "RightToe");
                BodyNode rF = new BodyNode(18, "RightFoot");
                BodyNode rL = new BodyNode(17, "RightLowerLeg");
                BodyNode rU = new BodyNode(16, "RightUpperLeg");
                rT.setRoot(rU);
                rF.setRoot(rU);
                rL.setRoot(rU);
                rU.setRoot(rU);

                rF.addNeighbour(rT);
                rL.addNeighbour(rF);
                rU.addNeighbour(rL);
                pelvis.addNeighbour(rU);

                // Also use the right uppper leg as orientation node.
                orientationNode = rU;
            }

            // Left arm
            {
                BodyNode lH = new BodyNode(15, "LeftHand");
                BodyNode lF = new BodyNode(14, "LeftForeArm");
                BodyNode lU = new BodyNode(13, "LeftUpperArm");
                BodyNode lS = new BodyNode(12, "LeftShoulder");
                lH.setRoot(lS);
                lF.setRoot(lS);
                lU.setRoot(lS);
                lS.setRoot(lS);

                lF.addNeighbour(lH);
                lU.addNeighbour(lF);
                lS.addNeighbour(lU);
                t8.addNeighbour(lS);
            }

            // Right arm
            {
                BodyNode rH = new BodyNode(11, "RightHand");
                BodyNode rF = new BodyNode(10, "RightForeArm");
                BodyNode rU = new BodyNode(9, "RightUpperArm");
                BodyNode rS = new BodyNode(8, "RightShoulder");
                rH.setRoot(rS);
                rF.setRoot(rS);
                rU.setRoot(rS);
                rS.setRoot(rS);

                rF.addNeighbour(rH);
                rU.addNeighbour(rF);
                rS.addNeighbour(rU);
                t8.addNeighbour(rS);
            }

            // Make the pelvis the root of the body.
            this.root = pelvis;
        }

    }
}
