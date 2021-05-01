# Training results

Training proceeded in the following steps:

First curriculum used was swing control. First starting with only swing control, no movement in the winch or crane movement.
Moving on to longer and longer lengths of cable to increase the difficulty. Also adding crane movement later on.

Second curriculum used was movement to a target. First moving only the cabin towards a location and giving a reward on hitting it. Swing was not enabled.
This had the unfortunate consequence of the AI only learning which way to travel. When the target was hit, the AI would just continue going in that direction.
This resulted in the next lesson, moving towards a target and staying there. This was done in a few steps. At first the AI still could only move the cabin. Hitting the target resulted in a stay time being set for the next time it would hit the target. This was increased every time the AI was succesful by 0.01 seconds up to a maximum of 5 seconds total.
Once the AI could stay succesfully in one direction, winch movement was allowed. Requiring the AI to also use the winch to get to the target. To aid in learning, the winch was limited to lower only up to the point of the target. While writing this, I realise that it would be better to limit the winch to a radius around the height of the target instead. In my implementation, the AI might have learned that it only needed to lower the winch. Limiting it to a radius around the target forces the AI to use up and down movement and can be increased in difficulty by increasing the radius.
After the AI learned to stay in 2 dimensions, I enabled the cable swing in order to teach it to control its cable swing now that it knew to go to a specific location. This did not go well. The model declined steadily resulting in an eventual -1f reward every time.