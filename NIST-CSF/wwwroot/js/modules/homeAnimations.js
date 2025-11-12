export class HomeAnimations {
    constructor() {
        this.cards = document.querySelectorAll('.home-card');
        this.init();
    }

    init() {
        this.addHoverEffects();
        this.addClickEffects();
        this.addKeyboardSupport();
    }

    addHoverEffects() {
        this.cards.forEach(card => {
            card.addEventListener('mouseenter', () => {
                card.style.transform = 'translateY(-8px)';
            });

            card.addEventListener('mouseleave', () => {
                card.style.transform = 'translateY(0)';
            });
        });
    }

    addClickEffects() {
        this.cards.forEach(card => {
            card.addEventListener('click', (e) => {
                // Efeito de clique
                card.style.transform = 'translateY(-2px) scale(0.98)';

                setTimeout(() => {
                    card.style.transform = 'translateY(-8px) scale(1)';
                }, 150);

                // Navegação
                const url = card.getAttribute('onclick')?.match(/'(.*?)'/)?.[1];
                if (url) {
                    setTimeout(() => {
                        window.location.href = url;
                    }, 300);
                }
            });
        });
    }

    addKeyboardSupport() {
        this.cards.forEach(card => {
            card.setAttribute('tabindex', '0');
            card.setAttribute('role', 'button');

            card.addEventListener('keypress', (e) => {
                if (e.key === 'Enter' || e.key === ' ') {
                    e.preventDefault();
                    card.click();
                }
            });
        });
    }
}