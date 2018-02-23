# C# Code Generation From ROS Messages
The application generates C# DTO classes from ROS .msg and .srv files with T4 Template. It also includes a client which can communicate with a ROS system through Rosbridge 2.0.

[![Build status](https://ci.appveyor.com/api/projects/status/yr51ut4a74veit33/branch/master?svg=true)](https://ci.appveyor.com/project/tothcs1105/roscodegenerationnglxen/branch/master)

## What is [ROS](http://www.ros.org/)?
ROS is an open-source, meta-operating system for your robot. It provides the services you would expect from an operating system, including hardware abstraction, low-level device control, implementation of commonly-used functionality, message-passing between processes, and package management. It also provides tools and libraries for obtaining, building, writing, and running code across multiple computers.

## What is [Rosbridge](http://wiki.ros.org/rosbridge_suite)?
Rosbridge provides a JSON API to ROS functionality for non-ROS programs. There are a variety of front ends that interface with rosbridge, including a WebSocket server for web browsers to interact with.
