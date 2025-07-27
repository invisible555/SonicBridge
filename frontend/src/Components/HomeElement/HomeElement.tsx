import React from 'react';
import type HomeElementProps from "./HomeElementProps";

const HomeElement: React.FC<HomeElementProps> = ({ title, description, image }) => {
    return (
        <div className="bg-white rounded-xl shadow-md overflow-hidden w-full flex flex-col items-center hover:shadow-lg transition-shadow">
            <img
                src={image}
                alt={title}
                className="w-24 h-24 mt-6 rounded-full object-cover shadow"
            />
            <div className="flex-1 flex flex-col justify-center items-center p-6 pt-4">
                <h5 className="text-xl font-semibold mb-2 text-gray-800 text-center">{title}</h5>
                <p className="text-gray-600 text-center">{description}</p>
            </div>
        </div>
    );
};

export default HomeElement;
