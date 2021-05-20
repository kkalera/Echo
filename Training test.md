# Training optimization.

The goal of this test is to optimize training parameters and number of environments for training.

## 1

9 environments
run_seed=2850

> Parameters:

    behaviors:
    Crane:
        trainer_type: ppo
        hyperparameters:
            batch_size: 512
            buffer_size: 4096
            learning_rate: 1e-3
            beta: 0.01
            epsilon: 0.2
            lambd: 0.95
            num_epoch: 6 #3
            learning_rate_schedule: linear
        network_settings:
            normalize: false
            hidden_units: 256
            num_layers: 3
            vis_encode_type: simple
        reward_signals:
            extrinsic:
                gamma: 0.99
                strength: 1.0
            curiosity:
                gamma: 0.99
                strength: 0.02
                encoding_size: 256
                learning_rate: 1e-3
        keep_checkpoints: 10
        checkpoint_interval: 500000
        max_steps: 1e05
        time_horizon: 64
        summary_freq: 10000
        threaded: true

## 2

9 environments
run_seed=2850

> Parameters:

    behaviors:
    Crane:
        trainer_type: ppo
        hyperparameters:
            batch_size: 512
            buffer_size: 4096
            learning_rate: 1e-3
            beta: 0.01
            epsilon: 0.2
            lambd: 0.95
            num_epoch: 8 #3
            learning_rate_schedule: linear
        network_settings:
            normalize: false
            hidden_units: 128
            num_layers: 2
            vis_encode_type: simple
        reward_signals:
            extrinsic:
                gamma: 0.99
                strength: 1.0
            curiosity:
                gamma: 0.99
                strength: 0.02
                encoding_size: 256
                learning_rate: 1e-3
        keep_checkpoints: 10
        checkpoint_interval: 500000
        max_steps: 1e05
        time_horizon: 128
        summary_freq: 10000
        threaded: true
