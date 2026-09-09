import axios from 'axios';

class ImageService {
    constructor() {
        this.items = [];
        this.timerId = setInterval(() => this.cleanup(), 60000);
    }

    async open(id) {
        var data = this;
        let item = data.items.find(i => i.id == id);
        if (!item) {
            await axios({
                url: `/home/download/${id}`,
                method: 'get',
                responseType: 'arraybuffer'
            }).then(function (response) {
                var bytes = new Uint8Array(response.data);
                item = { id, bytes, lastOpenTime: new Date() };
                data.items.push(item);
            }).catch(function (error) {
                throw error;
            });
        }
        else {
            item.lastOpenTime = new Date();
        }
        return URL.createObjectURL(new Blob([item.bytes]));
    }

    cleanup() {
        const now = new Date();
        this.items = this.items.filter(i => (now - i.lastOpenTime < 5 * 60000))
    }

    destroy() {
        clearInterval(this.timerId);
    }
}

export default new ImageService();