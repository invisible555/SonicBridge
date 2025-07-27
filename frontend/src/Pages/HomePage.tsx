import React from 'react';
import HomeElement from '../Components/HomeElement/HomeElement';

const homeData = [
    {
        title: "Szybkość działania",
        description: "Nasza aplikacja działa błyskawicznie na każdej przeglądarce.",
        image: "/speed.png"
    },
    {
        title: "Bezpieczeństwo",
        description: "Twoje dane są u nas bezpieczne - korzystamy z nowoczesnych zabezpieczeń.",
        image: "/security.png"
    },
    {
        title: "Intuicyjność",
        description: "Prosty i intuicyjny interfejs użytkownika.",
        image: "/intuition.png"
    },
    {
        title: "Wsparcie",
        description: "Szybkie wsparcie techniczne dla każdego użytkownika.",
        image: "/help.png"
    }
];

const HomePage: React.FC = () => {
    return (
        <div className="w-full px-4 mt-8">
            <h1 className="text-4xl font-semibold mb-8 text-gray-900 text-center">Nasze Atuty</h1>
            <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-8">
                {homeData.map((item, index) => (
                    <HomeElement
                        key={index}
                        title={item.title}
                        description={item.description}
                        image={item.image}
                    />
                ))}
            </div>
        </div>
    );
};
export default HomePage;
