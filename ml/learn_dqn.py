import numpy as np
import gym
from gym.spaces.utils import flatdim
import gym_card_game

from keras.models import Sequential
from keras.layers import Dense, Activation, Flatten, Input, Reshape
from keras.optimizers import Adam

from rl.agents.dqn import DQNAgent
from rl.policy import BoltzmannQPolicy
from rl.memory import SequentialMemory


ENV_NAME = 'card_game-v0'


# Get the environment and extract the number of actions.
env = gym.make(ENV_NAME)
# env = FlattenDictWrapper(env)
# np.random.seed(123)
# env.seed(123)
nb_actions = env.action_space.n



# Next, we build a very simple model.
# print (env.observation_space.shape)
obs_dims = flatdim(env.observation_space)

model = Sequential()
# model.add(Flatten(input_shape=(1, obs_dims)))
model.add(Dense(32, input_shape=(None,obs_dims)))
model.add(Dense(16))
model.add(Activation('relu'))
model.add(Dense(16))
model.add(Activation('relu'))
model.add(Dense(nb_actions))
model.add(Reshape((nb_actions,)))
# model.add(Activation('linear'))
print(model.summary())

# Finally, we configure and compile our agent. You can use every built-in Keras optimizer and
# even the metrics!
memory = SequentialMemory(limit=20, window_length=1)
policy = BoltzmannQPolicy()
dqn = DQNAgent(model=model, nb_actions=nb_actions, memory=memory, nb_steps_warmup=10,
               target_model_update=1e-2, policy=policy)
dqn.compile(Adam(lr=1e-3), metrics=['mae'])


dqn.fit(env, nb_steps=20, visualize=True, verbose=2)


dqn.save_weights('dqn_{}_weights.h5f'.format(ENV_NAME), overwrite=True)

# Finally, evaluate our algorithm for 5 episodes.
# dqn.test(env, nb_episodes=5, visualize=True)
