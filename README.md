# Unity3DGridTest

A tower defense prototype developed in one week.

![Game Screenshot](https://github.com/itismarcii/Unity3DGridTest/assets/58007324/d1fc076f-1b93-4173-96e2-21f7172c7e0a)
![Game Screenshot](https://github.com/itismarcii/Unity3DGridTest/assets/58007324/4c0cda90-fa96-4901-8e4c-a563c279f5b7)

## Disclaimer
Some features are not yet implemented:
- Continue
- Load
- Data Storage
- Menu Settings
- Quick Menu

## Implemented Features
- **Custom Map Generator**: Create maps with dimensions from 5x5 to 35x35, including non-square formats like 16x25.
- **Basic Tower Defense Logic**: Foundation for core tower defense mechanics.
- **Enemy and Building Management**: Easily manage enemies and buildings with range indicators.
- **Level and Wave Customization**: Use scriptable objects to tailor levels and waves to your liking.

![Custom Map Generator](https://github.com/itismarcii/Unity3DGridTest/assets/58007324/3afde358-16eb-4d0e-93fa-ff8259780ef9)
![Enemy and Building Management](https://github.com/itismarcii/Unity3DGridTest/assets/58007324/9036f0b0-8999-482b-bb65-38b1ab560d9f)
![Level and Wave Customization](https://github.com/itismarcii/Unity3DGridTest/assets/58007324/9d8940cb-57b5-4cab-85fc-4b14dc31af94)

## Usage
### Controls
- **UI Selection**: Tap to select.
- **Placing a Building**: Tap to place.
- **Removing a Building**: Hold touch to remove.

### Getting Started
1. **Set Grid Dimensions**: Input your desired X and Y dimensions.
2. **Create Your World**: Click and drag tiles from the right panel onto your grid. Note: Ensure at least one spawn and finish tile are connected, directly or via path tiles. Unconfigured tiles will default to building spaces.
3. **System Tabs**: 
    - **Left**: World Scene (leaves every window)
    - **Middle**: Work In Progress (WIP)
    - **Right**: Building Menu
4. **Building Menu**: 
    - Left: Base turret
    - Middle: High turret (effective against flying enemies)
    - Right: Double turret
5. **Placing Buildings**: Select a building and place it on the grid if you have enough gold (costs are not yet displayed).
6. **Removing Buildings**: Select a placed building and hold touch to remove it (no refund).
7. **Start Waves**: Begin waves when ready.
8. **Adjust Speed**: Use the speed control in the top right to adjust game speed.
9. **Break Between Waves**: Take breaks between waves as needed.
10. **Enjoy the Game!**
