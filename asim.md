Two types of games

Data Stated  Games : Games that can be represented and update soley as states/data. Gameplay/Visuals/Story progresses is centered around updating the states. E.g Chess

Simulated Games : Games that uses simulation Gameplay/Visuals/Story and progress is centered around having a system/engine to simulated do updates.

Data Stated Games needs to have
1. UpdateState()
2. VerifyState()
2a FullStateVerification - verify the entire state
    requires rollbackState()
2b PartialStateVerification - verify only updated changes of state
    use for when game needs to perform animation for failed verification
3. SaveStateToDisk() - optional to save to disk
4. LoadDisplayState()
5. VerifyWinState() - verify win condition
6. EndGame() - end the game