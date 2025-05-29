export  class GameBoard {
    app = null
    #refTable = null;
    #hub = null
    #userId = null
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
        if(playerId === player1["id"]) {
            this.drawPlayerTag(player1["nickName"], player2["nickName"])
        }else{
            this.drawPlayerTag(player2["nickName"], player1["nickName"])
        }
        
        this.drawQuitGameButton()
        
        let nextToPlay = game["nextToPlay"] 
        let btnActive = nextToPlay>=0 ? game["players"][nextToPlay]["id"] === playerId : false
        
        this.drawTurnButton(btnActive)
        this.drawTurnTimer(game["turnTime"])
    }
    
    drawTurnButton(active){
        if(!!this.turnButton){
            this.updateTurnButton(active);
            return
        }
        
        let padding = 5
        let vh = this.app.screen.height
        let vw = this.app.screen.width
        
        const buttonConfig = {
            width: 100,
            height: 40,
            radius: 10,
            fillColor: 0xd2d2d2,
            hoverColor: 0x2952fc,
            textColor: 0x222222,
            textSize: 12,
            padding: 6,
            text: ""
        };

        const btn = new Button(buttonConfig, this.app);
        this.turnButton = btn;
        btn.onClick(()=> this.#EndTurn())
        this.updateTurnButton(active);
        btn.setPosition(vw - padding - buttonConfig.width, vh/2 - buttonConfig.height);
        btn.render()
    }
    
    drawTurnTimer(time) {
        console.log(time);

        const config = {
            color: 0xd2d2d2,
            textSize: 20,
            text: time
        };
        this.turnTimer = new TurnTimer(config,this.app)
    }
    
    updateTurnTimer(time)
    {
        this.turnTimer.update(time);
    }
    
    updateTurnButton(active){
        let btn = this.turnButton ;
        if(!btn){return}
        btn.setActive(active)
        if(active){
            btn.setText("End turn")
            btn.buttonContainer.cursor = 'pointer'
        }else{
            btn.setText("Opponent turn")
            btn.buttonContainer.cursor = 'default'
        } 
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
            textSize: 14,
            padding: 6,
            text: 'Quit'
        };
        
        const btn = new Button(buttonConfig, this.app);
        btn.buttonContainer.cursor = 'pointer'
        btn.setActive(true)
        btn.onClick(()=> this.#OnQuit())
        btn.setPosition(vw - padding - buttonConfig.width, padding)
        btn.render()

    }
    
    async #GetUserId (){
        if(!this.#userId){
            this.#userId = await this.#refTable.table["match"].invokeMethodAsync("GetUserId")
        }
        
       return this.#userId 
    }
    
    async #EndTurn() {
        if(this.endTurnPending){return}
        this.endTurnPending = true;
        let userId = await this.#GetUserId() 
        if (!userId) {
            console.log("LoggedOut")
            return
        }

        await window.endTurn()
        this.endTurnPending = false;
    }
    async #OnQuit(){
        if (this.quitPending) { return }
        this.quitPending = true
        let userId = await this.#GetUserId()
        if (!userId) {
            console.log("LoggedOut")
            return
        }

        await window.leaveGame()
        this.quitPending = false
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

    
}

class TurnTimer {
    app = null
    textBox = null

    constructor(config,app) {
        this.app = app;

        let padding = 5
        let vh = this.app.screen.height
        let vw = this.app.screen.width

        this.textBox = new PIXI.HTMLText({
            text: `<span>${config.text}<span>` ,
            style: {
                fontFamily: 'DM Sans',
                fontSize: config.textSize,
                fill: config.color,
            },
        });
        this.textBox.position.set(vw-padding- this.textBox.width -40 , vh/2 - 40 - this.textBox.height - 5 , 1);
        this.app.stage.addChild(this.textBox)
    }

    update(time) {
        this.textBox.text =`<span>${time}<span>`
    }
}

class Button {
    config = null
    #ClickObservers = []
    active = false
    buttonText = null

    constructor(config,app) {
        this.config = config;
        this.active = config.active;
        this.app = app;
        this.buttonContainer = new PIXI.Container();
        this.buttonText = new PIXI.Text({
            text: this.config.text,
            style: new PIXI.TextStyle({
                fontSize: config.textSize ?? 16,
                fontFamily: "Verdana",
                fontStyle: "italic",
                fontWeight: "bold",
                padding: this.config.padding
            })
        });
    }

    onClick(callback) {
        this.#ClickObservers.push(callback);
    }

    render(container){
        if(!this.buttonText) {return;}
        // Create button graphics
        const button = new PIXI.Graphics()
            .roundRect(0, 0, this.config.width, this.config.height, this.config.radius)
            .fill(this.config.fillColor);


        // Center the text
        this.buttonText.anchor.set(0.5);
        this.buttonText.position.set(this.config.width / 2, (this.config.height + this.config.padding * 2) / 2);
        this.scaleTextToFit()

        // Create container for button elements
        this.buttonContainer.addChild(button, this.buttonText);
        this.buttonContainer.eventMode = 'static'; // Make interactive

        // Add hover effects
        this.buttonContainer.on('pointerover', () => {
            button.clear().roundRect(0, 0, this.config.width, this.config.height, this.config.radius)
                .fill(this.active?this.config.hoverColor:this.config.fillColor);
            this.buttonText.style.fill = this.active? this.config.fillColor : this.config.textColor;
        });

        // remover hover effect
        this.buttonContainer.on('pointerout', () => {
            button.clear().roundRect(0, 0, this.config.width, this.config.height, this.config.radius)
                .fill(this.config.fillColor);
            this.buttonText.style.fill = this.config.textColor
        });

        // Add click handler
        this.buttonContainer.on('pointerdown', async () => {
            if(!this.active){return}
            for (const cb of this.#ClickObservers) {
                cb()
            }
        });

        if (container){
            container.addChild(this.buttonContainer)
            return
        }

        this.app.stage.addChild(this.buttonContainer)
    }

    setActive(active){
        this.active = active
    }

    setText(content){
        this.buttonText.text = content
        this.scaleTextToFit()
    }

    scaleTextToFit() {
        this.#scaleTextToFit(this.buttonText,this.config.width,this.config.height)
    }

    #scaleTextToFit(text, w, h) {
        // Reduce font size until text fits vertically
        while (text.height > (h * 6 / 10) && text.style.fontSize > 4) { // 70 = rect height - padding
            text.style.fontSize--;
        }
    }

    setPosition(x,y) {
        this.buttonContainer.position.set(x, y);
    }
}