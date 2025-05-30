export class APIRequest {
    static URL = "http://localhost:5002"

    // Cookie should be set on successful login 
    // so just return a bool
    static async login(username, password) {
        const response = await fetch(`${APIRequest.URL}/login`, {
            method: "POST",
            credentials: 'include',
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({
                username: username,
                password: password,
            })
        });
        return response.ok;
    }

    // Cookie should be removed on successful logout 
    // So just return a bool
    static async logout() {
        const response = await fetch(`${APIRequest.URL}/logout`, {
            credentials: 'include'
        });
        return response.ok;
    }

    // Return a bool so that i can redirect the user on success.
    // Assuming tha anything other than a 2xx is due to a connection credential
    // being wrong.
    static async register(username, password, email, nickname) {
        const response = await fetch(`${APIRequest.URL}/register`, {
            method: "POST",
            credentials: 'include',
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({
                username: username,
                password: password,
                email: email,
                nickname: nickname
            })
        });

        return {
            IsSuccess: response.ok,
            ErrorMessage: await response.text()
        };
    }
}
