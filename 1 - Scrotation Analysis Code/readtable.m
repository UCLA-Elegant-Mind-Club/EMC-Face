function t = readtable(filename, varargin)
    [varargin{1:2:end}] = convertStringsToChars(varargin{1:2:end});
    names = varargin(1:2:end);
    try
        if any(strcmpi(names,"Format"))
            t = matlab.io.internal.legacyReadtable(filename,varargin);
        else
            func = matlab.io.internal.functions.FunctionStore.getFunctionByName('readtable');
            C = onCleanup(@()func.WorkSheet.clear());
            t = func.validateAndExecute(filename,varargin{:});
        end
    catch ME
        throw(ME)
    end

    try
        global ignoreCol;
        if ignoreCol>0
            matrix = table2array(t);
            matrix = [matrix(:,1:ignoreCol-1), matrix(:, ignoreCol+1:end)];
            matrix = matrix(find(matrix(:,1)~=420.69),:);
            matrix = readXControl.processMatrix(matrix);
            names = t.Properties.VariableNames;
            t = array2table(matrix, "VariableNames", [names(:,1:ignoreCol-1), names(:, ignoreCol+1:end)]);
        end
    catch; end
end