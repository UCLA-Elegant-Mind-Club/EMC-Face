%% Data Analysis Script for Eye Tracking Protocol Data

% Benjamin Asdell, 2/13/2022
% Mark Diamond, 2/13/2022 updates
    % WORKING!!!!!

% Updated on 6/30/2022 for Sentence Reading Analysis
% Even more updated on 7/12/2022 for actually working stuff lol hopefully

% July-August 2022:
    % Updated several times by Mark and the Attention Group
    % to make degree plots with labels, fixing broken plots, and overlaying
    % sentence text behind X-Y plot.

% 8/9/2022:
    % Discovered and fixed process error to match up with the max 10
    % seconds protocol timeout, instead of using maxReadingTime to cut the data.

% 8/9/2022:
    % Added animated X-Y plot

close all; clear all; clc;
%% Loading Data from Files

[EyeDataName, EyeDataPath] = uigetfile('*.txt'); %opens selection dialog box to select specific eye data
fullEyeDataFilename = fullfile(EyeDataPath, EyeDataName); %concatenates path and filename
rawEyeTrackingData = readtable(fullEyeDataFilename); %FIX?: Imports everything, TobiiState Present/NotPresent imports as NaN

rawEyeTrackingData = rawEyeTrackingData(:,2:end); % Removes first column
rawEyeTrackingData(any(ismissing(rawEyeTrackingData), 2), :) = []; % Removes NaN entries
rawEyeTrackingData = table2array(rawEyeTrackingData); %Convert to array

[RTDataName, RTDataPath] = uigetfile('*.csv'); %opens selection dialog box to select specific trial data
fullRTDataFilename = fullfile(RTDataPath, RTDataName); %concatenates path and filename
RTData = readtable(fullRTDataFilename); %reads all protocol data to table
RTData = table2array(RTData); %Convert to array


%% Processing

breakpoints = RTData(:,3); %finds specific timestamps where face appears
maxReadingTime = round(max(RTData(:,2))*1000); % In Milliseconds, rounded to whole
rawEyeTracking_Time = rawEyeTrackingData(:,1);
rawEyeTracking_X = rawEyeTrackingData(:,2);
rawEyeTracking_Y = rawEyeTrackingData(:,3);

A = repmat(rawEyeTracking_Time, [1, height(breakpoints)]);
[minValue, closestIndex] = min(abs(A-breakpoints'));
closestValue = rawEyeTracking_X(closestIndex); % Not needed, just interesting, cool to see that theyre all ~2000 px (middle of screen)!

EyeTracking_X = zeros(1000+1, length(closestIndex)); % 1000 for max 10 second timeout in protocol
EyeTracking_Y = zeros(1000+1, length(closestIndex));
EyeTracking_Time = zeros(1000+1, length(closestIndex));

%% Plotting

figure(1)

% X-Y Plot Overlay
figure(2)
img = imread('Pasta.png');
imshow(img);

for i = 1:length(closestIndex)
    EyeTracking_X(:,i) = rawEyeTracking_X(closestIndex(i):closestIndex(i)+1000);
    EyeTracking_Y(:,i) = rawEyeTracking_Y(closestIndex(i):closestIndex(i)+1000);
    EyeTracking_Time(:,i) = rawEyeTracking_Time(closestIndex(i):closestIndex(i)+1000) - rawEyeTracking_Time(closestIndex(i));
    
    B = EyeTracking_Time;
    [minValue2, closestIndex2] = min(abs(B-maxReadingTime'));
    
    hold off
    figure(1)
    hold on
    plot(EyeTracking_Time(:,i), EyeTracking_X(:,i))
    title("X-Time Plot")
    xlabel("Time (s)")
    ylabel("Horizontal Distance (degrees)")
    xlim([0,maxReadingTime]) % Time, in s
    ylim([-3.2053, 3843.21])
    xticks([0,1000,2000,3000,4000,5000,6000,7000,8000,9000,10000])
    xticklabels([0,1,2,3,4,5,6,7,8,9,10])
    yticks([-3.2053,637.863,1278.93,1920,2561.07,3202.14,3843.21])
    yticklabels([-30,-20,-10,0,10,20,30])
    

    hold off
    figure(2)
    hold on
    % 150:max to cut off jumbled mess before the trial begins after cue
    plot(EyeTracking_X(1:max(closestIndex2),i), EyeTracking_Y(1:max(closestIndex2),i), "LineWidth", 2, "Marker", '.', "MarkerSize", 20)
    title("X-Y Plot")
    set(gca, 'YDir','reverse')
    xlabel("Horizontal eccentricity (degrees)")
    ylabel("Vertical eccentricity (degrees)")
    axis([-3.2053 3843.21 -115.351 2275.35]) % For X_Y Plot
    xticks([-3.2053,637.863,1278.93,1920,2561.07,3202.14,3843.21])
    xticklabels([-30,-20,-10,0,10,20,30])
    yticks([-115.351, 482.324, 1080, 1677.68, 2275.35])
    yticklabels([-20,-10,0,10,20])
    

    hold off
    figure(3)
    hold on
    plot(EyeTracking_Time(:,i), EyeTracking_Y(:,i))
    set(gca, 'YDir','reverse')
    title("Y-Time Plot")
    xlabel("Time (s)")
    ylabel("Vertical Distance (degrees)")
    xlim([0,maxReadingTime]) % Time, in s
    ylim([-115.351, 2275.35])
    xticks([0, 1000, 2000, 3000, 4000, 5000,6000,7000,8000,9000,10000])
    xticklabels([0, 1, 2, 3, 4, 5,6,7,8,9,10])
    yticks([-115.351, 482.324, 1080, 1677.68, 2275.35])
    yticklabels([-20,-10,0,10,20])
end