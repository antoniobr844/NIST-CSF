export class BreadcrumbManager {
    constructor(breadcrumbSelector = '.breadcrumb') {
        this.breadcrumb = document.querySelector(breadcrumbSelector);
        this.items = this.breadcrumb ? Array.from(this.breadcrumb.querySelectorAll('div')) : [];
    }

    update(step) {
        if (!this.breadcrumb) return;

        // Remover classe active de todos
        this.items.forEach(item => item.classList.remove('active'));

        // Ativar até o step atual
        for (let i = 0; i < step && i < this.items.length; i++) {
            this.items[i].classList.add('active');
        }
    }

    setText(step, text) {
        if (this.items[step - 1]) {
            this.items[step - 1].textContent = text;
        }
    }

    reset() {
        this.items.forEach(item => item.classList.remove('active'));
        if (this.items[0]) {
            this.items[0].classList.add('active');
        }
    }
}