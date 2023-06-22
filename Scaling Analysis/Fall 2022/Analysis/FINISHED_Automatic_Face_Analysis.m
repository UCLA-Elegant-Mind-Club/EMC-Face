%% Data Analysis Script for Eye Tracking Protocol Data
% #sponsored by Mark and Daniel (special thanks to Caominh and Brian)
close all; clear all; clc;

%% Declaring Variables
% monitor dimensions

width_px = 4096;
height_px = 2160;
width_cm = 121;
height_cm = width_cm * height_px / width_px;

%participant distance
dist_cm = 121;

% Eye tracker delay in ms (includes monitor delay)
eyeTrackDelay = 0;

% see plotIndex function at bottom for indexing info of plotData
% plotData also prepares images
clear plotData;
global plotData;
plotData(100).eyeTrackingLines = [];
plotData(1).target = [];

%% Loading and Processing Data from Files

% Import and reformat raw eye-tracking data from txt file
[file, folder] = uigetfile('*.txt');
eyeData = readtable(fullfile(folder, file));
eyeData = eyeData(~any(ismissing(eyeData).'), :); % Remove NaN rows
eyeTime = eyeData{:,2}; % CPU uptime in ms
eyeX = atand((eyeData{:,3} - width_px/2) / width_px * width_cm / dist_cm); %x and y coords are now in angles of eccen
eyeY = atand((eyeData{:,4} - height_px/2) / height_px * height_cm / dist_cm);

% Import and reformat RT data from csv file
[file, folder] = uigetfile('*.csv');
RTData = table2array(readtable(fullfile(folder, file)));
breakPoints = RTData(:,5) + eyeTrackDelay; %finds CPU uptime when face appears in ms (1000 ms delay in eyetracker)
endPoints = breakPoints + RTData(:,3); %finds CPU uptime when response in ms
targets = RTData(:,4);
heights = RTData(:,2);

%% Processing
eyeTrackingIndex = 1;
for trial = 1:height(breakPoints)
    while eyeTime(eyeTrackingIndex) < breakPoints(trial)
        eyeTrackingIndex = eyeTrackingIndex + 1;
    end
    start = eyeTrackingIndex;
    while eyeTime(eyeTrackingIndex) < endPoints(trial)
        eyeTrackingIndex = eyeTrackingIndex + 1;
    end
    index = plotIndex(targets(trial), heights(trial), RTData(trial,1));
    if index > 0
        plotData(index).eyeTrackingLines = [plotData(index).eyeTrackingLines; [start, eyeTrackingIndex - 1]];
    end
end

%% Plotting
for i = 1:length(plotData)
    if ~isempty(plotData(i).target)
        %plotXY(eyeX, eyeY, plotData(i), false);
        plotXTime(eyeX, eyeTime, plotData(i));
        %plotYTime(eyeY, eyeTime, plotData(i));
    end
end

%%% MAIN CODE STOPS HERE %%%
%%% MAIN CODE STOPS HERE %%%
%%% MAIN CODE STOPS HERE %%%
%%% MAIN CODE STOPS HERE %%%
%%% MAIN CODE STOPS HERE %%%

%% Functions
function index = plotIndex(target, size, correct)
    if nargin < 3; correct = 1; end
    if target > 3; target = mod(target - 1, 3) + 1; end
    imgs = ["face 1.png", "face 2.png", "face 3.png", "121cm Dots.png"];
    targets = [1,2,3,-1];
    sizes = [1, 1.41, 2, 2.83, 4, 5.66, 8, 11.31, 16, 21.17, 28];

    global plotData;
    targetIndex = find(targets == target);
    sizeIndex = find(sizes == size);

    if target == -1
        index = 1; % Callib dots currently disabled idk what img
        size = 27.8; % Screenshot, so height = atand(height_cm / dist_cm)
    elseif correct == 420.69
        index = targetIndex + 1;
        sizeIndex = 0;
    elseif correct == 0
        index = -1;
    else
        index = 1 + length(targets) + (targetIndex-1) * length(sizes) + sizeIndex;
    end

    if index > 0 && isempty(plotData(index).target)
        plotData(index).target = target;
        plotData(index).targetIndex = targetIndex;
        plotData(index).size = size;
        plotData(index).sizeIndex = sizeIndex;
        plotData(index).img = imread(imgs(targetIndex));
    end
end

function prepSubplot(plotData, type)
    if nargin < 2; type = 0; end
    rows = 2; cols = 3; total = 12; % 11 sizes + 1 trainer
    if plotData.target == -1
        fig = figure(type + 1);
        set(fig, 'Name', "Calibration Dots");
    else
        figNum = floor(plotData.sizeIndex / rows / cols) + 2;
        figNum = (plotData.targetIndex - 1) * ceil(total / rows / cols) + figNum;
        fig = figure(figNum * 10 + type);
        set(fig, 'Name', "Target " + plotData.target);
        subplot(2, 3, mod(plotData.sizeIndex, rows * cols) + 1);
    end
end

function plotXY(eyeX, eyeY, plotData, showLines)
    if nargin < 5; showLines = false; end
    prepSubplot(plotData, 0);

    bound = plotData.size/2;
    bound2 = bound / height(plotData.img) * width(plotData.img);
    imshow(plotData.img, 'YData', [-bound bound], 'XData', [-bound2 bound2]);
    set(gca, 'color', 'k', 'box','off');
    set(gca, 'YDir','reverse')
    axis on;
    title(plotData.size + " degree height");
    %xlabel("Horizontal Eccentricity (degrees)");
    %ylabel("Vertical Eccentricity (degrees)");
    
    hold on
    for i = 1:height(plotData.eyeTrackingLines)
        range = plotData.eyeTrackingLines(i,1):plotData.eyeTrackingLines(i,2);
        if showLines; plot(eyeX(range), eyeY(range), "LineWidth", 2, "Marker", '.', "MarkerSize", 20);
        else; scatter(eyeX(range), eyeY(range), 30, "Marker", '.'); end
    end
    axis([-bound2 bound2 -bound bound]);
end

function plotXTime(eyeX, eyeTime, plotData)
    prepSubplot(plotData, 1);

    title(plotData.size + " degree height");
    %xlabel("Time (ms)");
    %ylabel("Horizontal Eccentricity (degrees)");
    
    hold on;
    for i = 1:height(plotData.eyeTrackingLines)
        startTime = eyeTime(plotData.eyeTrackingLines(i,1));
        range = plotData.eyeTrackingLines(i,1):plotData.eyeTrackingLines(i,2);
        plot(eyeTime(range) - startTime, eyeX(range));
    end
    bound = plotData.size/2 / height(plotData.img) * width(plotData.img);
    axis([0, inf, -bound, bound]) % Automatically rescales x-axis: Time, in ms
end

function plotYTime(eyeY, eyeTime, plotData)
    prepSubplot(plotData, 2);

    title(plotData.size + " degree height");
    %xlabel("Time (ms)")
    %ylabel("Vertical Eccentricity (degrees)")
    set(gca, 'YDir','reverse')
    
    hold on
    for i = 1:height(plotData.eyeTrackingLines)
        startTime = eyeTime(plotData.eyeTrackingLines(i,1));
        range = plotData.eyeTrackingLines(i,1):plotData.eyeTrackingLines(i,2);
        plot(eyeTime(range) - startTime, eyeY(range));
    end
    axis([0, inf, -plotData.size/2, plotData.size/2]) % Automatically rescales x-axis: Time, in ms
end