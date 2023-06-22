%% Brought to you by Daniel&Caominh Inc.
close all;

%% Reading Data from Files
%{
clear all; clc;

% Eye tracking file
[file,folder]=uigetfile('*.csv', 'Select Eye Tracking File');
ETdata=readtable(fullfile(folder,file),"Delimiter",",");
eyeTime=ETdata{:,1};
combinedGazeVector = cellfun(@(x)x(2:end-1),ETdata{:,4},'uniformoutput',false);
combinedGazeVector = str2double(split(combinedGazeVector,","));
normX=combinedGazeVector(:,1);
normY=combinedGazeVector(:,2);
normZ=combinedGazeVector(:,3);
eyeX = atand(normX./normZ);
eyeY = atand(normY./normZ);
%}
% RT file
[file, folder] = uigetfile('*.xlsx', 'Select Reaction Time File');
RTData = readtable(fullfile(folder, file));
eyeTrackDelay = 150; % Eye tracker delay in ms
breakPoints = RTData{:,1} + eyeTrackDelay;
endPoints = RTData{:,2} + eyeTrackDelay;
targets = RTData{:,3};

%targets = arrayfun(@(x)find(x==["Face1","Face2","Face3"]),targets+"");

heights = ones(height(RTData{:,2}),1) * 16;
%heights = round(atand(RTData{:,2} / 20) * 2);
%heights = RTData{:,2};

%}


%% Calibration
global calibOnly;
calibOnly = false;

% Eye data is shifted after dilation factors are applied
stretchX = 1;
stretchY = 1;
deltaX = 0;
deltaY = 0;

eyeX = atand(normX./normZ) * stretchX + deltaX;
eyeY = atand(normY./normZ) * stretchY + deltaY;

%% see plotIndex function at bottom for indexing info of plotData
% plotData also prepares images
global plotData;
clear('global', "plotData");
global plotData;
plotData(100).eyeTrackingLines = [];
plotData(1).target = [];

%% Processing
eyeTrackingIndex = 1;
for trial = 1:height(breakPoints)
    while eyeTime(eyeTrackingIndex) < breakPoints(trial)
        eyeTrackingIndex = eyeTrackingIndex + 1;
    end
    start = eyeTrackingIndex;
    while eyeTrackingIndex < length(eyeTime) && eyeTime(eyeTrackingIndex) < endPoints(trial)
        eyeTrackingIndex = eyeTrackingIndex + 1;
    end
    index = plotIndex(targets(trial), heights(trial), RTData{trial,1});
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
    imgs = ["face 1.png", "face 2.png", "face 3.png", "abomination.jpg"];
    targets = [0,1,2,3,-1];
    sizes = [1,2,4,8,16];

    global plotData, global calibOnly;
    targetIndex = find(targets == target);
    sizeIndex = find(sizes == size);

    if target == -1
        index = 1;
        size = 8; % ref size
    elseif calibOnly
        index = -1;
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
    rows = 1; cols = 2; total = 4; % 1 size + no trainer
    if plotData.target == -1
        fig = figure(type + 1);
        set(fig, 'Name', "Calibration Dots");
    else
        figNum = floor(plotData.sizeIndex / rows / cols) + 2;
        figNum = (plotData.targetIndex - 1) * ceil(total / rows / cols) + figNum;
        fig = figure(figNum * 10 + type);
        set(fig, 'Name', "Target " + plotData.target);
        subplot(rows, cols, mod(plotData.sizeIndex, rows * cols) + 1);
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
    title(plotData.size + " Degree Height (Nyla)");
    xlabel("Horizontal Eccentricity (degrees)");
    ylabel("Vertical Eccentricity (degrees)");
    
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

    title(plotData.size + " Degree Height");
    xlabel("Time (ms)");
    ylabel("Horizontal Eccentricity (degrees)");
    
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

    title(plotData.size + " Degree Height");
    xlabel("Time (ms)")
    ylabel("Vertical Eccentricity (degrees)")
    set(gca, 'YDir','reverse')
    
    hold on
    for i = 1:height(plotData.eyeTrackingLines)
        startTime = eyeTime(plotData.eyeTrackingLines(i,1));
        range = plotData.eyeTrackingLines(i,1):plotData.eyeTrackingLines(i,2);
        plot(eyeTime(range) - startTime, eyeY(range));
    end
    axis([0, inf, -plotData.size/2, plotData.size/2]) % Automatically rescales x-axis: Time, in ms
end