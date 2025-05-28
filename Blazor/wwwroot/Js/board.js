export  class GameBoard {
    app = null
    #refTable = null;
    #hub = null
    constructor(hub,refTable) {
        if (!hub.connection) {
            console.error("Missing connection");
            return;
        }
        this.#hub = hub;
        this.#refTable = refTable;
    }
    async init(container_name) {
        this.app = new PIXI.Application();
        await this.app.init({ background: '#222222', resizeTo: window });
        let board = document.querySelector(`#${container_name}`)
        if (!!board) {
            board.appendChild(this.app.view)
        }
    }

    drawBoard(game,playerId) {
        let player1 = game["players"][0] 
        let player2 = game["players"][1]
        console.log(player1, player2)
        if(playerId === player1["id"]) {
            this.drawPlayerTag(player1["nickName"], player2["nickName"])
        }else{
            this.drawPlayerTag(player2["nickName"], player1["nickName"])
        }
        this.drawQuitGameButton()
    }

    drawQuitGameButton() {

        let padding = 5
        let vh = this.app.screen.height
        let vw = this.app.screen.width

        // Button configuration
        const buttonConfig = {
            width: 100,
            height: 30,
            radius: 10,
            fillColor: 0xd2d2d2,
            hoverColor: 0x2952fc,
            textColor: 0x222222,
            padding: 2,
            text: 'Quit'
        };

        // Create button graphics
        const button = new PIXI.Graphics()
            .roundRect(0, 0, buttonConfig.width, buttonConfig.height, buttonConfig.radius)
            .fill(buttonConfig.fillColor);

        // Create button text
        const buttonText = new PIXI.Text({
            text: buttonConfig.text,
            style: new PIXI.TextStyle({
                fontSize: 16,
                fontFamily: "Verdana",
                fontStyle: "italic",
                fontWeight: "bold",
                padding: buttonConfig.padding
            })
        });

        // Center the text
        buttonText.anchor.set(0.5);
        buttonText.position.set(buttonConfig.width / 2, (buttonConfig.height + buttonConfig.padding * 2) / 2);

        // Create container for button elements
        const buttonContainer = new PIXI.Container();
        buttonContainer.addChild(button, buttonText);
        buttonContainer.position.set(vw - padding - buttonConfig.width, padding);
        buttonContainer.eventMode = 'static'; // Make interactive
        buttonContainer.cursor = 'pointer'

        // Add hover effects
        buttonContainer.on('pointerover', () => {
            button.clear().roundRect(0, 0, buttonConfig.width, buttonConfig.height, buttonConfig.radius)
                .fill(buttonConfig.hoverColor);
            buttonText.style.fill = buttonConfig.fillColor
        });

        buttonContainer.on('pointerout', () => {
            button.clear().roundRect(0, 0, buttonConfig.width, buttonConfig.height, buttonConfig.radius)
                .fill(buttonConfig.fillColor);
            buttonText.style.fill = buttonConfig.textColor
        });

        // Add click handler
        buttonContainer.on('pointerdown', async () => {
            if (this.quitPending) { return }
            this.quitPending = true
            let userId = await this.#refTable.table["match"].invokeMethodAsync("GetUserId")
            if (!userId) {
                console.log("LoggedOut")
                return
            }

            await window.leaveGame(userId)
            this.quitPending = false
        });

        this.app.stage.addChild(buttonContainer)

    }

    drawPlayerTag(top_player_name, bottom_player_name) {

        let padding = 25
        let w = 100
        let h = 30

        let config = {
            x: padding,
            y: padding,
            width: 100,
            height: 30,
            fillColor: 0xd2d2d2,
            textColor: 0x222222,
            padding: 10,
            radius: 5
        }
        try {
            this.#drawTag(top_player_name, config)
            config.y = this.app.screen.height - config.height - padding
            this.#drawTag(bottom_player_name, config)
        } catch (error) {
            console.log(error)
        }
    }

    #drawTag(name, config) {
        const tag = new PIXI.Graphics()
            .roundRect(0, 0, config.width, config.height, config.radius)
            .fill(config.fillColor);
        // Create button text
        const text = new PIXI.Text({
            text: name,
            style: new PIXI.TextStyle({
                fontSize: 20,
                fontFamily: "Verdana",
                fontStyle: "italic",
                fontWeight: 500,
                padding: config.padding
            })
        });

        const container = new PIXI.Container();
        container.position.set(config.x, config.y, config.width, config.height);
        //text.position.set((config.width) / 2, (config.height) / 2);

        this.#scaleTextToFit(text, config.width, config.height)
        this.#centerText(text, config)
        container.addChild(tag, text);
        this.app.stage.addChild(container)
    }

    #scaleTextToFit(text, w, h) {
        // Reduce font size until text fits vertically
        while (text.height > (h * 6 / 10) && text.style.fontSize > 4) { // 70 = rect height - padding
            text.style.fontSize--;
        }
    }

    #centerText(text, config) {

        text.anchor.set(0.5);
        text.position.set((config.width + config.padding) / 2, (config.height + config.padding * 2) / 2);
    }

    async readyToPlay()
    {
        let conn = this.#hub.connection
        if (conn == null) { return }
        return await conn.invoke("ReadyToPlayAsync")
    }
}
